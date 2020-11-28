(async function () {
    const main = {};
    main.conversationId = document.getElementById('conversation-id').textContent;
    main.userId = await fetch('/Profiles/WhoAmI?').then(r => r.json())
    main.messagesList = document.getElementById("messages-list");
    main.messagesCount = document.getElementById('messages-count');
    main.messageTextArea = document.getElementById("message-textarea");
    main.requestVerificationToken = document.querySelector('*[name=__RequestVerificationToken]').value


    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    //Disable send button until connection is established
    document.getElementById("send-button").disabled = true;

    async function receiveAndLoadMessageAsync({id, conversationId}) {
        if (main.conversationId !== conversationId) {
            main.messagesCount.textContent = (1 + +main.messagesCount.textContent) + ''
            return;
        }

        const {content, senderUserName, createdOn, senderId} = await fetch(`/Conversations/GetMessage/${id}`)
            .then(r => r.json())
        const messageListItem = createMessage(id, {content, senderUserName, createdOn, senderId});
        main.messagesList.appendChild(messageListItem);
        main.messagesList.scrollTop = main.messagesList.scrollHeight;
    }
    
    function loadMessage({id, content, senderUserName, createdOn, senderId}){
        const messageListItem = createMessage(id, {content, senderUserName, createdOn, senderId});
        main.messagesList.firstElementChild.insertAdjacentElement('afterend', messageListItem);
    }
    
    function createMessage(id, {content, senderUserName, createdOn, senderId}){
        const isSender = main.userId === senderId;
        const li = document.createElement("li");
        li.innerHTML = `
${isSender ? '<div class="col"></div>' : ''}
<div class="col-auto">
    <div class="container">
        <div class="row">
            ${isSender ? '<div class="col"></div>' : ''}
            <small class="col-auto">
                ${senderUserName} at
                <span>${new Date(createdOn).toLocaleString()}</span>
            </small>
            ${!isSender ? '<div class="col"></div>' : ''}
        </div>
        <div class="row">
            <div class="col">
                <p class="p-2 border rounded text-white ${isSender ? "bg-secondary" : "bg-info"}">
                    ${content}
                </p>
            </div>
        </div>
    </div>
</div>
${!isSender ? '<div class="col"></div>' : ''}
        `;
        li.id = id;
        li.classList.add('row', 'm-3');
        return li;
    }

    await fetch(`/Conversations/GetMessagesPage?conversationId=${main.conversationId}`)
        .then(r => r.json())
        .then(messages => messages.forEach(message => loadMessage(message)));

    connection.on("ReceiveMessage", receiveAndLoadMessageAsync);
    connection
        .start()
        .then(() => document.getElementById("send-button").disabled = false)
        .catch((err) => console.error(err.toString()));

    document.getElementById('load-older-button')
        .addEventListener('click', async function () {
            await fetch(`/Conversations/GetMessagesPage?` +
                `conversationId=${main.conversationId}&` +
                `referenceMessageId=${main.messagesList.firstElementChild.nextElementSibling.id}`)
                .then(r => r.json())
                .then(messages => messages.forEach(message => loadMessage(message)));
        })

    document.getElementById('send-button')
        .addEventListener('click', async function (event) {
            event.preventDefault();
            const content = main.messageTextArea.value;

            const formData = new FormData();
            formData.set('content', content);
            formData.set('conversationId', main.conversationId);
            formData.set('__RequestVerificationToken', main.requestVerificationToken);

            await fetch('/Conversations/SendMessage/', {
                method: 'post',
                body: formData
            })
                .then(r => r.ok ? main.messageTextArea.value = '' : null)
        });

})()

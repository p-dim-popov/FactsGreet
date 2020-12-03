(async function () {
    const main = {};
    main.userId = await fetch('/Profiles/WhoAmI?').then(r => r.json())
    main.conversationsList = document.getElementById("conversations-list");
    main.requestVerificationToken = document.querySelector('*[name=__RequestVerificationToken]').value
    main.searchForConversation = '';
    main.searchForUser = '';
    main.groupsList = document.getElementById('groups-list');
    main.usersList = document.getElementById('user-emails-list')

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    async function receiveAndLoadConversationInfoAsync({id, conversationId}) {
        const {content, senderUserName, createdOn, senderId} = await fetch(`/Conversations/GetMessage/${id}`)
            .then(r => r.json())

        const title = await fetch(`/Conversations/GetConversationTitle/${conversationId}`)
            .then(r => r.json())
        document.getElementById(conversationId)?.remove();

        const conversationInfoListItem = createConversationInfo(conversationId, {
            title,
            content,
            senderUserName,
            createdOn,
            senderId
        });

        main.conversationsList.firstElementChild.insertAdjacentElement('beforebegin', conversationInfoListItem);
    }

    function loadConversationInfo({id, title, message: {content, senderUserName, createdOn, senderId}}) {
        const conversationInfoListItem = createConversationInfo(id, {
            title,
            content,
            senderUserName,
            createdOn,
            senderId
        });

        main.conversationsList.lastElementChild.insertAdjacentElement('beforebegin', conversationInfoListItem);
    }

    function createConversationInfo(id, {title, content, senderUserName, createdOn, senderId}) {
        const isSender = main.userId === senderId;
        const li = document.createElement("li");
        li.innerHTML = `
<div class="card" style="width: 100%">
    <div class="card-body">
        <h5 class="card-title font-weight-bold">${title}</h5>
            <p class="card-text">
                <b>${isSender ? 'You' : senderUserName}: </b> 
                ${content.substring(0, 100)} ${content.length < 100 ? '' : '...'}
            </p>
            <small>${new Date(createdOn).toLocaleString()}</small>
            <a class="stretched-link" href="/Conversations/Messages?id=${id}"></a>
    </div>
</div>
        `;
        li.id = id;
        li.classList.add('row', 'm-3', 'border', 'rounded');
        li.setAttribute('data-space-time-continuum', new Date(createdOn).toISOString()); //just in case for order
        return li;
    }

    await fetch(`/Conversations/GetConversationsPage`)
        .then(r => r.json())
        .then(conversationInfos => conversationInfos.forEach(conversationInfo => loadConversationInfo(conversationInfo)));

    connection.on("ReceiveMessage", receiveAndLoadConversationInfoAsync);
    connection.start().catch((err) => console.error(err.toString()));

    document.getElementById('load-older-button')
        .addEventListener('click', async function () {
            await fetch(`/Conversations/GetConversationsPage?` +
                `referenceConversationId=${main.conversationsList.lastElementChild.previousElementSibling.id}`)
                .then(r => r.json())
                .then(messages => messages.forEach(message => loadConversationInfo(message)));
        })
    
    setInterval(async function (){
        const searchForConversation = document.getElementById('group-participants-input').value;
        if (main.searchForConversation === searchForConversation) return;
        main.searchForConversation = searchForConversation;

        [...main.groupsList.children].forEach(c => c.remove());
        if (main.searchForConversation === '') return;
        
        const conversations = await fetch(`/Conversations/GetConversationsIdsByEmails?users=${main.searchForConversation}`)
            .then(r => r.json());
        
        conversations.forEach(({id, title}) => {
            const link = document.createElement('a');
            link.href = `/Conversations/Messages?id=${id}`;
            link.textContent = title;
            link.classList.add('list-group-item')
            main.groupsList.appendChild(link);
        })
    }, 500);

    setInterval(async function (){
        if (!!main.actionIsPending) return;
        main.actionIsPending = true;
        const searchForUser = document.getElementById('user-email-input').value;
        if (main.searchForUser === searchForUser) return;
        main.searchForUser = searchForUser;

        [...main.usersList.children].forEach(c => c.remove());
        if (main.searchForUser === '') return;

        const users = await fetch(`/Profiles/Get10EmailsByEmailKeyword?keyword=${main.searchForUser}`)
            .then(r => r.json());

        users.forEach((email) => {
            const link = document.createElement('a');
            link.href = `/Conversations/Messages?email=${email}`;
            link.textContent = email;
            link.classList.add('list-group-item')
            main.usersList.appendChild(link);
        })
        
        main.actionIsPending = false;
    }, 500);
})()

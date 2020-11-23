(function () {
    $(document).ready(function () {
        // set editor content
        $('#mergely').mergely({
            cmsettings: {
                readOnly: false,
                lineWrapping: true
            },
            wrap_lines: true,

            //Doesn't do anything?
            autoresize: true,

            editor_width: 'calc(50% - 25px)',
            editor_height: '100%',

            lhs: function (setValue) {
                setValue(document.getElementById('target-textarea').value);
            },
            rhs: function (setValue) {
                setValue(document.getElementById('against-textarea').value);
            }
        });
    });

    window.targetPages = window.againstPages = {'<': 1, '>': 1};
    const [targetSelect, againstSelect] = [document.getElementById('target-select'), document.getElementById('against-select')];

    [targetSelect, againstSelect]
        .forEach(el => el.addEventListener('change', async function (e) {
            const selectedOption = e.target[e.target.selectedIndex];

            let referenceOption;
            let position;
            if (selectedOption.value === '<') {
                referenceOption = selectedOption.nextElementSibling;
                position = 'beforebegin';
            } else if (selectedOption.value === '>') {
                referenceOption = selectedOption.previousElementSibling;
                position = 'afterend';
            } else return;

            const page = e.target.id === 'target-select' ? 'targetPages' : 'againstPages';

            try {
                const response =
                    await fetch(`/Edits/${window.selectOptionsActionName}/` +
                        `${referenceOption.value}?` +
                        `which=${selectedOption.value}&` +
                        `page=${window[page][selectedOption.value]++}`);

                let data = await response.json();

                if (selectedOption.value === '<')
                    data = data.reverse();

                data
                    .map(d => {
                        const option = document.createElement('option');
                        option.value = d.id;
                        option.textContent = `${d.createdOn} | ${d.editorUserName} | ${d.comment}`
                        return option;
                    })
                    .forEach(el => referenceOption.insertAdjacentElement(position, el))
            } catch (e) {
            }

            e.target.selectedIndex = 1;
        }));

    [...document.getElementsByClassName('go-btn')]
        .forEach(el => el.addEventListener('click', async function (e) {
            window.location.href = `/Edits/View/` +
                `${targetSelect[targetSelect.selectedIndex].value}?` +
                `against=${againstSelect[againstSelect.selectedIndex].value}`;
        }))
})()

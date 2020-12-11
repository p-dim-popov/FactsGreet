(function () {
    createDiffViews();
    prepareEditsNavigation();

    function createDiffViews(viewType) {
        viewType = viewType || 0;
        const byId = (id) => document.getElementById(id),
            base = difflib.stringAsLines(byId("target-textarea").value),
            newtxt = difflib.stringAsLines(byId("against-textarea").value),
            sm = new difflib.SequenceMatcher(base, newtxt),
            opcodes = sm.get_opcodes(),
            diffoutputdiv = byId("diffoutput");

        diffoutputdiv.appendChild(diffview.buildView({
            baseTextLines: base,
            newTextLines: newtxt,
            opcodes: opcodes,
            baseTextName: "Target",
            newTextName: "Against",
            contextSize: null,
            viewType: viewType
        }));
    }

    function prepareEditsNavigation() {
        window.targetPages = {'<': 1, '>': 1};
        window.againstPages = {'<': 1, '>': 1};

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
                            `page=${window[page][selectedOption.value]}`);

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

                    window[page][selectedOption.value]++
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
    }
})()

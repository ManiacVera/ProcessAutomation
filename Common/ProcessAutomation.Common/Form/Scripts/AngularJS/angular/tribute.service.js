// tribute.service.js
app.factory('tributeService', function () {
    let variables = [];
    let functions = [];

    const createCollection = function (trigger, source, selectHandler) {
        return {
            trigger: trigger,

            values: function (text, cb) {
                let filtrados = source().filter(x =>
                    x.key.toLowerCase().includes(text.toLowerCase())
                );

                cb(filtrados);
            },

            menuItemTemplate: function (item) {
                return `
                    <div class="tribute-item">
                        <span>${item.original.key}</span>
                    </div>
                `;
            },

            selectTemplate: selectHandler
        };
    };

    const tribute = new Tribute({
        collection: [
            createCollection('@', () => variables, function (item) {
                return `[@${item.original.value}]`;
            }),

            createCollection('$', () => functions, function (item) {
                if (item.original.value.startsWith("[$"))
                    return `${item.original.value}`;
                else
                    return `[{ ${item.original.value} }]`;
            }),

            createCollection('!', () => functions, function (item) {
                if (item.original.value.startsWith("[$"))
                    return `${item.original.value}`;
                else
                    return `${item.original.value}`;
            })
        ]
    });

    return {
        tribute: tribute,

        setVariables: function (array) {
            variables = array;
        },

        setFunctions: function (array) {
            functions = array;
        }
    };
});
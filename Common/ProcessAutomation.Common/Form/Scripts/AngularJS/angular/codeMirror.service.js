
app.factory('codeMirrorService', ['$document', '$rootScope', function ($document, $rootScope) {

    let activeInstance = null;
    let activeElement = null;
    let activeNgModel = null;
    let activeScope = null;

    let currentLanguage = 'text/x-csharp';
    let globalDisabled = false;

    let variables = [];
    let functions = [];

    let toggleButton = null;
    let resizeObserver = null;

    const service = {
        setLanguage: setLanguage,
        getLanguage: getLanguage,
        setDisabled: setDisabled,
        isDisabled: isDisabled,

        setVariables: function (array) {
            variables = array || [];
        },

        setFunctions: function (array) {
            functions = array || [];
        },

        attach: attach,
        detach: detach,
        destroyInstance: destroyInstance,

        getActiveInstance: function () {
            return activeInstance;
        },

        getActiveElement: function () {
            return activeElement;
        },

        isActiveElement: function (el) {
            return activeElement === el;
        }
    };

    createToggleButton();

    angular.element(window).on('scroll resize', positionToggleButton);

    return service;

    function setLanguage(lang) {

        currentLanguage = lang;

        if (activeInstance) {
            activeInstance.setOption('mode', currentLanguage);
        }
    }

    function getLanguage() {
        return currentLanguage;
    }

    function setDisabled(disabled) {

        globalDisabled = !!disabled;

        if (globalDisabled) {
            destroyInstance();
        } else {
            createInstance();
        }

        updateToggleButton();
        updateToggleVisibility();
    }

    function isDisabled() {
        return globalDisabled;
    }

    function attach(element, ngModel, scope) {

        const targetElement = element[0];

        if (activeElement === targetElement) {

            activeNgModel = ngModel;
            activeScope = scope;

            if (!globalDisabled && !activeInstance) {
                createInstance();
            }

            updateToggleVisibility();
            positionToggleButton();

            return;
        }

        if (activeInstance) {
            destroyInstance();
        }

        activeElement = targetElement;
        activeNgModel = ngModel;
        activeScope = scope;

        if (activeNgModel) {
            activeElement.value = activeNgModel.$viewValue == null
                ? ''
                : activeNgModel.$viewValue;
        }

        if (!globalDisabled) {
            createInstance();
        }

        updateToggleVisibility();
        positionToggleButton();
    }

    function detach(element) {

        const targetElement = element[0];

        if (activeElement !== targetElement) {
            return;
        }

        destroyInstance();

        activeElement = null;
        activeNgModel = null;
        activeScope = null;

        updateToggleVisibility();
    }

    function createInstance() {

        if (globalDisabled || !activeElement || activeInstance) {
            return;
        }

        if (activeNgModel) {
            activeElement.value = activeNgModel.$viewValue == null
                ? ''
                : activeNgModel.$viewValue;
        }

        const textareaHeight = activeElement.offsetHeight;
        const textareaWidth = activeElement.offsetWidth;

        activeInstance = CodeMirror.fromTextArea(activeElement, {

            lineNumbers: true,
            mode: currentLanguage,
            theme: 'default',
            indentUnit: 4,
            smartIndent: true,
            lineWrapping: false,

            extraKeys: {
                'Ctrl-Space': 'autocomplete'
            }
        });

        activeInstance.setSize(textareaWidth, textareaHeight);

        activeInstance.on('change', function (cm) {

            if (!activeNgModel) {
                return;
            }

            const val = cm.getValue();

            if (activeNgModel.$viewValue === val) {
                return;
            }

            const model = activeNgModel;
            const scope = activeScope;

            if (scope && !scope.$$phase && !scope.$root.$$phase) {

                scope.$apply(function () {
                    model.$setViewValue(val);
                });

            } else {
                model.$setViewValue(val);
            }
        });

        activeInstance.on('inputRead', function (cm, change) {

            if (!change.text || !change.text.length) {
                return;
            }

            const cursor = cm.getCursor();
            const line = cm.getLine(cursor.line);
            const textBefore = line.slice(0, cursor.ch);
            const match = textBefore.match(/[@$!][a-zA-Z0-9_]*/);

            if (!match) {
                return;
            }

            const trigger = match[0][0];

            if (match.index + match[0].length !== cursor.ch) {
                return;
            }

            showAutocomplete(cm, trigger);
        });

        activeInstance.refresh();

        observeResize();

        updateToggleVisibility();
        positionToggleButton();
    }

    function destroyInstance() {

        if (!activeInstance) {
            return;
        }

        const instance = activeInstance;
        const element = activeElement;
        const model = activeNgModel;
        const scope = activeScope;
        const value = instance.getValue();

        activeInstance = null;

        if (resizeObserver) {
            resizeObserver.disconnect();
            resizeObserver = null;
        }

        if (model && model.$viewValue !== value) {

            if (scope && !scope.$$phase && !scope.$root.$$phase) {

                scope.$apply(function () {
                    model.$setViewValue(value);
                });

            } else {
                model.$setViewValue(value);
            }
        }

        instance.toTextArea();

        if (element) {
            element.value = value;
        }
    }

    function createToggleButton() {

        if (toggleButton) {
            return;
        }

        toggleButton = angular.element(
            '<button type="button" class="cm-global-toggle" title="">' +
                '<i class="fa fa-code">&lt;/&gt;</i>' +
            '</button>'
        );

        toggleButton.css({
            position: 'absolute',
            zIndex: 99999,
            display: 'none'
        });

        toggleButton.on('mousedown', function (event) {
            event.preventDefault();
        });

        toggleButton.on('click', function (event) {

            event.preventDefault();
            event.stopPropagation();

            $rootScope.$applyAsync(function () {
                setDisabled(!globalDisabled);
            });
        });

        angular.element(document.body).append(toggleButton);
    }

    function updateToggleVisibility() {

        if (!toggleButton) {
            return;
        }

        updateToggleButton();

        if (!activeElement) {
            toggleButton.css('display', 'none');
            return;
        }

        positionToggleButton();

        toggleButton.css('display', 'flex');
    }

    function positionToggleButton() {

        if (!toggleButton || !activeElement) {
            return;
        }

        const target = activeInstance
            ? activeInstance.getWrapperElement()
            : activeElement;

        const rect = target.getBoundingClientRect();

        toggleButton.css({
            top: (rect.top + window.pageYOffset + 5) + 'px',
            left: (rect.right + window.pageXOffset - 46) + 'px'
        });
    }

    function observeResize() {

        if (resizeObserver) {
            resizeObserver.disconnect();
            resizeObserver = null;
        }

        if (!window.ResizeObserver) {
            return;
        }

        const target = activeInstance
            ? activeInstance.getWrapperElement()
            : activeElement;

        if (!target) {
            return;
        }

        resizeObserver = new ResizeObserver(function () {
            positionToggleButton();
        });

        resizeObserver.observe(target);
    }

    function updateToggleButton() {

        if (!toggleButton) {
            return;
        }

        toggleButton.toggleClass('cm-disabled', globalDisabled);

        toggleButton.attr(
            'title',
            globalDisabled ? 'Enable CodeMirror' : 'Disable CodeMirror'
        );
    }

    function showAutocomplete(cm, trigger) {

        if (!CodeMirror.showHint) {
            return;
        }

        if (cm.state.completionActive) {
            cm.state.completionActive.close();
        }

        CodeMirror.showHint(cm, function (editor) {

            const cursor = editor.getCursor();
            const line = editor.getLine(cursor.line);
            const textBefore = line.slice(0, cursor.ch);

            const match = textBefore.match(/[@$!][a-zA-Z0-9_]*$/);

            if (!match || match[0][0] !== trigger) {
                return null;
            }

            const typedText = match[0].substring(1).toLowerCase();

            const source = trigger === '@' ? variables : functions;

            const list = source
                .filter(function (item) {
                    return item.key &&
                        item.key.toLowerCase().includes(typedText);
                })
                .map(function (item) {

                    let insertText;

                    if (trigger === '@') {
                        insertText = `[@${item.value}]`;
                    } else if (trigger === '$') {
                        insertText = item.value.startsWith('[$')
                            ? item.value
                            : `[{ ${item.value} }]`;
                    } else {
                        insertText = item.value;
                    }

                    return {
                        text: insertText,
                        displayText: item.key,
                        data: item
                    };
                });

            return {
                list: list,
                from: CodeMirror.Pos(
                    cursor.line,
                    cursor.ch - match[0].length
                ),
                to: cursor
            };

        }, {
            completeSingle: false,
            closeOnUnfocus: true
        });
    }

}]);

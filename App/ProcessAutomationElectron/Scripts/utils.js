const store = {
    set: (k, v) => window.electronAPI.setGlobal(k, v),
    get: (k) => window.electronAPI.getGlobal(k)
};

const params = new URLSearchParams(window.location.search);

function getObjectsFromComboBoxes(querySelector, includeEmpty, includeDuplicates) {
    const combos = document.querySelectorAll(querySelector); 
    const result = [];
    
    combos.forEach(select => {
        if (select.value !== "" || includeEmpty) {
            const value = select.value;
            const text = select.options[select.selectedIndex].text;

            if (includeDuplicates) {
                result.push({ Value: value, Text: text });
            } else {
                const exists = result.some(item => item.Value === value);
                if (!exists) {
                    result.push({ Value: value, Text: text });
                }
            }
        }
    });
    
    return result;
}

function getObjectsFromTextBoxes(querySelector) {
    const textboxes = document.querySelectorAll(querySelector);
    const uniqueValues = new Set();

    textboxes.forEach(input => {
        const value = input.value.trim();
        if (value !== "") {
            uniqueValues.add(value);
        }
    });

    return Array.from(uniqueValues).map(value => ({
        Value: value,
        Text: value
    }));
}

function TagIt(objectId) {
    if (objectId != null) {
        $('#' + objectId).tagit({
            allowSpaces: true
        });
    }
}

function GetPositionElement(elementId) {
    var position = $('#' + elementId).offset();

    return {
        "x": position.left,
        "y": position.top
    };
}

function GetAttributeElement(elementId, attribute) {
    return $('#' + elementId).attr(attribute);
}

function GetCssElement(elementId, css) {
    return $('#' + elementId).css(css);
}

function SetAccordion(elementId) {
    $("#" + elementId).accordion();
}

function SetScrollToBottom(elementId) {
    var element = document.getElementById(elementId);

    if (element != null) {
        element.scrollTop = element.scrollHeight;
    }
}

function SetElementValue(elementId, value) {
    var element = document.getElementById(elementId);

    if (element != null) {
        element.value = value;
    }
}

function SetFullWidth(elementId, substractWidth) {
    var element = document.getElementById(elementId);

    if (element != null) {
        var body = document.body,
            html = document.documentElement;

        var width = Math.max(body.scrollWidth, body.offsetWidth,
            html.clientWidth, html.scrollWidth, html.offsetWidth);

        // width -= element.offsetRigth;

        if (substractWidth) {
            width -= (substractWidth);
        }

        element.style.width = width.toString() + "px";
    }
}

function SetFullHeight(elementId, substractHeight) {
    var element = document.getElementById(elementId);

    if (element != null) {
        var body = document.body,
            html = document.documentElement;

        var height = Math.max(body.scrollHeight, body.offsetHeight,
            html.clientHeight, html.scrollHeight, html.offsetHeight);

        height -= element.offsetTop;

        if (substractHeight) {
            height -= (substractHeight);
        }

        element.style.height = height.toString() + "px";
    }
}

function SetFullScreenHeight(elementId, substractHeight) {
    var element = document.getElementById(elementId);

    if (element != null) {
        var body = document.body,
            html = document.documentElement;

        var height = window.screen.height;

        height -= element.offsetTop;

        if (substractHeight) {
            height -= (substractHeight);
        }

        element.style.height = height.toString() + "px";
    }
}

function GetIframeWindow(iframeId) {
    var doc;
    var iframeObject = document.getElementById(iframeId);

    if (iframeObject.contentWindow) {
        return iframeObject.contentWindow;
    }

    if (iframeObject.window) {
        return iframeObject.window;
    }

    if (!doc && iframeObject.contentDocument) {
        doc = iframeObject.contentDocument;
    }

    if (!doc && iframeObject.document) {
        doc = iframeObject.document;
    }

    if (doc && doc.defaultView) {
        return doc.defaultView;
    }

    if (doc && doc.parentWindow) {
        return doc.parentWindow;
    }

    return null;
}

function SetTranslation(iframeId, fieldTranslationList) {
    setTimeout(function () {
        var iframeElement = GetIframeWindow(iframeId);
        if (iframeElement) {
            if (typeof iframeElement.setTranslation === 'function') {
                iframeElement.setTranslation(fieldTranslationList);
            }
            else {
                alert("Error al cargar funcion de setTranslation");
            }
        }
    }, 500);
}

function SetInputFields(iframeId, fieldName, componentName, getSetSelected, fieldArray, variableArray) {
    setTimeout(function () {
        var iframeElement = GetIframeWindow(iframeId);
        if (iframeElement) {
            if (typeof iframeElement.setFields === 'function') {
                iframeElement.setFields(fieldName, componentName, getSetSelected, fieldArray, variableArray);
            }
            else {
                alert("Error al cargar funcion de setInputFieldArray");
            }
            }
    }, 500);
}

function SetInputFieldsOptions(iframeId, enabledOptions) {
    setTimeout(function () {
        var iframeElement = GetIframeWindow(iframeId);
        if (iframeElement) {
            if (typeof iframeElement.setEnabledOptions === 'function') {
                iframeElement.setEnabledOptions(enabledOptions);
            }
            else {
                alert("Error al cargar funcion de SetInputFieldsOptions");
            }
        }
    }, 500);
}

function SetOutputValueFields(iframeId, fieldName, componentName, token, fieldArray) {
    setTimeout(function () {
        var iframeElement = GetIframeWindow(iframeId);
        if (iframeElement) {
            if (typeof iframeElement.setFields === 'function') {
                iframeElement.setFields(fieldName, componentName, token, fieldArray);
            }
            else {
                alert("Error al cargar funcion de setInputFieldArray");
            }
            }
    }, 500);
}

function SetStyle(iframeId) {
    setTimeout(function () {
        var iframeElement = GetIframeWindow(iframeId);
        if (iframeElement) {
            if (typeof iframeElement.setStyle === 'function') {
                iframeElement.setStyle();
            }
            else {
                alert("Error al cargar funcion de SetStyle");
            }
        }
    }, 500);
}

function OnInputSuccessData(iframeId) {
    var result = "";
    var iframeElement = GetIframeWindow(iframeId);
    if (iframeElement) {
        if (typeof iframeElement.onSuccessData === 'function') {
            result = iframeElement.onSuccessData();
        }
        else {
            alert("Error al cargar funcion de onSuccessData");
        }
    }
    return result;
}

function SaveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

function ShowLoading()
{
    if ($("#loadingPopup")) {
        $("#loadingPopup").modal('show');
    }
}

function CloseLoading()
{
    if ($('#loadingPopup')) {
        $('#loadingPopup').modal('hide');
    }
}

function OpenTab(url)
{
    const link = document.createElement('a');
    link.href = url;
    link.target = '_blank';
    document.body.appendChild(link);
    link.click();
    link.remove();
}

function AlertMessage(message)
{
    alert(message);
}

function CopyToClipboard(elementId)
{
    var element = document.getElementById(elementId);
    if (element)
    {
        element.select();
        document.execCommand("copy");
    }
}

function CopyToClipboardWitAlert(elementId)
{
    var element = document.getElementById(elementId);
    if (element)
    {
        element.select();
        document.execCommand("copy");
        alert("Copied to clipboard");
    }
}

function retrieveStepVariableList(stepNumber, taskModel, processData) {
    let variableList = [];

    variableList.push(
        ...taskModel.StepList
            .filter(step => step.Number < stepNumber && step.Variable && step.Variable !== "")
            .sort((a, b) => a.Variable.localeCompare(b.Variable))
            .map(step => step.Variable)
    );

    variableList.push(
        ...processData.TaskList
            .filter(task => task.Number < taskModel.Number)
            .sort((a, b) => b.Number - a.Number)
            .flatMap(task =>
                task.StepList
                    .filter(step => step.IsPublicVariable && step.Variable && step.Variable !== "")
                    .map(step => `${step.Variable},${task.Name}`)
            )
    );

    return [...new Set(variableList)].sort((a, b) => a.localeCompare(b));
}

function getTableDataByMap(tableId, map) {
  const table = document.getElementById(tableId);
  if (!table) return [];

  const rows = table.querySelectorAll("tbody tr");
  const result = [];

  rows.forEach(row => {
    const rowData = {};

    Object.entries(map).forEach(([key, config]) => {
      const selector = typeof config === "string" ? config : config.selector;
      const type = typeof config === "string" ? "auto" : config.type;

      const element = row.querySelector(selector);
      if (!element) return;

      let value;

      if (type === "auto") {

        if (element.type === "checkbox") {
          value = element.checked;

        } else if (element.type === "number") {
          value = element.value === "" ? null : Number(element.value);

        } else {
          value = element.value ?? element.textContent.trim();

          if (!isNaN(value) && value.trim() !== "") {
            value = Number(value);
          }
        }

      } else if (type === "number") {
        value = Number(element.value ?? element.textContent.trim());

      } else if (type === "boolean") {
        value = element.checked;

      } else {
        value = element.value ?? element.textContent.trim();
      }

      rowData[key] = value;
    });

    if (Object.keys(rowData).length > 0) {
      result.push(rowData);
    }
  });

  return result;
}

function getElementValue(element) {
    const tag = element.tagName.toUpperCase();

    if (tag === 'INPUT') {
        const type = element.type.toLowerCase();
        if (type === 'checkbox' || type === 'radio') {
            return element.checked ? element.value : null;
        }
        return element.value;
    }

    if (tag === 'TEXTAREA' || tag === 'SELECT') {
        return element.value;
    }
    
    return element.textContent.trim();
}

function setTableFieldByString(tableId, fieldCompare, stringValue, fieldSelector, newValue) {
    const table = document.getElementById(tableId);
    if (!table) return false;

    const tbody = table.querySelector('tbody');
    if (!tbody) return false;

    const row = Array.from(tbody.rows).find(r => {
        const element = r.querySelector(fieldCompare);
        if (!element) return false;

        const value = getElementValue(element);
        return String(value).trim() === String(stringValue);
    });

    if (!row) return false;

    const field = row.querySelector(fieldSelector);
    if (!field) return false;

    const tag = field.tagName.toUpperCase();

    if (tag === 'INPUT') {
        const type = field.type.toLowerCase();
        if (type === 'checkbox' || type === 'radio') {
            field.checked = Boolean(newValue);
        } else {
            field.value = newValue;
        }
    } else if (tag === 'TEXTAREA' || tag === 'SELECT') {
        field.value = newValue;
    } else {
        field.textContent = newValue;
    }

    return true;
}

function findRowByColumnValue(tableId, searchSelector, searchValue, map) {
  const table = document.getElementById(tableId);
  if (!table) return null;

  const rows = table.querySelectorAll("tbody tr");

  for (const row of rows) {
    const searchElement = row.querySelector(searchSelector);
    if (!searchElement) continue;

    let cellValue = searchElement.value ?? searchElement.textContent.trim();

    if (searchElement.type === "checkbox") {
      cellValue = searchElement.checked;
    }

    if (String(cellValue) === String(searchValue)) {
      const rowData = {};

      Object.entries(map).forEach(([key, config]) => {
        const selector = typeof config === "string" ? config : config.selector;
        const type = typeof config === "string" ? "auto" : config.type;

        const element = row.querySelector(selector);
        if (!element) return;

        let value;

        if (type === "auto") {
          if (element.type === "checkbox") {
            value = element.checked;
          } else if (element.type === "number") {
            value = element.value === "" ? null : Number(element.value);
          } else {
            value = element.value ?? element.textContent.trim();
          }
        } else if (type === "number") {
          value = Number(element.value ?? element.textContent.trim());
        } else if (type === "boolean") {
          value = element.checked;
        } else {
          value = element.value ?? element.textContent.trim();
        }

        rowData[key] = value;
      });

      return rowData;
    }
  }

  return null;
}

function getNextId(list, idField) {
  const ids = list
    .map(obj => obj[idField])
    .filter(id => typeof id === "number")
    .sort((a, b) => a - b);

  let newId = 1;

  for (let id of ids) {
    if (id === newId) {
      newId++;
    } else if (id > newId) {
      break;
    }
  }

  return newId;
}

function sleep(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}
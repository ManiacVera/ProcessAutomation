let i18n;
let resultInfo = {};
let page = "";
let itemCode = "";

async function initApp() {
  await window.electronAPI.i18nInit('InputPage', 'es-MX');

  i18n = {
    t: window.electronAPI.i18nT
  };

  await changeLanguage('es');
  await loadPage();
}

window.addEventListener('DOMContentLoaded', initApp);

async function changeLanguage(lang) {
  await window.electronAPI.i18nInit('InputPage', lang);
  document.getElementById('btnAccept').innerText = await i18n.t('Accept');
  document.getElementById('btnCancel').innerText = await i18n.t('Cancel');
}

async function loadPage() {
}

function toBase64(str) {
    const bytes = new TextEncoder().encode(str);
    let binary = "";

    bytes.forEach(b => binary += String.fromCharCode(b));

    return btoa(binary);
}

var inputData = null;
window.electronAPI.onPopupData(async (data) => {
  inputData = data;
  page = data.Page;
  itemCode = data.ItemCode;
  resultInfo = await window.api.buildInputFormat(data);
  if (resultInfo.Ok)
  {
      if (resultInfo.ResolvedPage == "InputTextField.html")
      {
          document.getElementById('ifrmInputFields').src = "../Resources/InputTextField.html";
      }

      SetTranslation("ifrmInputFields", resultInfo.FieldTranslation);

      SetInputFields( "ifrmInputFields", resultInfo.Description, resultInfo.Component,
        resultInfo.ValueAction, resultInfo.InputFormatText, resultInfo.VariableArray
      );

      if (resultInfo.ResolvedPage != "InputTextField.html")
      {
        SetInputFieldsOptions("ifrmInputFields", data.EnabledOptions);
      }

      SetStyle("ifrmInputFields");
  }
  else
  {
      closeModal();
  }
});

async function btnAcceptClick() {
  const result = OnInputSuccessData("ifrmInputFields");
  resultInfo.Result = result;

  let resultMessage = null;
  let formError = false;

  try {
      resultMessage = JSON.parse(resultInfo.Result);

      if (resultMessage && resultMessage.hasErrorInForm !== undefined 
        && Boolean(resultMessage.hasErrorInForm)) {
          formError = true;
      }
  } catch (ex) {
  }

  if (formError) {
      await window.electronAPI.showAlert("Error al capturar campos");

  } else {
      if (resultInfo.ResolvedPage != "InputTextField.html")
      {
        inputData.Value = toBase64(resultInfo.Result);
        resultInfo.Result = await window.api.buildResultValueFormat(inputData);
      }

      closeModal(resultInfo);
  }
}

function btnCancelClick() {
  closeModal();
}

function closeModal(data) {
  if (data) {
    data.Page = page;
    data.ItemCode = itemCode;
  }

  window.electronAPI.sendPopupResult(data);
  window.close();
}

document.querySelector('#btnAccept').addEventListener('click', btnAcceptClick);
document.querySelector('#btnCancel').addEventListener('click', btnCancelClick);
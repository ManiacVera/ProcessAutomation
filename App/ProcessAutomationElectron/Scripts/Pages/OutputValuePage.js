let i18n;
let resultInfo = {};
let page = "";
let itemCode = "";

async function initApp() {
  await window.electronAPI.i18nInit('OutputValuePage', 'es');

  i18n = {
    t: window.electronAPI.i18nT
  };

  await changeLanguage('es');
  await loadPage();
}

window.addEventListener('DOMContentLoaded', initApp);

async function changeLanguage(lang) {
  await window.electronAPI.i18nInit('OutputValuePage', lang);
  document.getElementById('btnAccept').innerText = await i18n.t('Accept');
  document.getElementById('btnCancel').innerText = await i18n.t('Cancel');
}

async function loadPage() {
}

window.electronAPI.onPopupData(async (data) => {
  page = data.Page;
  itemCode = data.ItemCode;

  SetOutputValueFields("ifrmOutputValue", data.Description, data.Component, data.Token, data.Value);
});

async function btnAcceptClick() {
  const result = OnInputSuccessData("ifrmOutputValue");
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
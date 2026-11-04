const i18next = require('i18next');
const fs = require('fs');
const path = require('path');

function initI18n(pageName, lang = 'es') {
  const pagePath = path.join(__dirname, '..', 'locales', lang, `${pageName}.json`);
  const pageTranslations = JSON.parse(fs.readFileSync(pagePath, 'utf8'));

  i18next.init({
    lng: lang,
    fallbackLng: 'es',
    resources: {
      [lang]: {
        translation: pageTranslations
      }
    },
    ns: ['translation'],
    defaultNS: 'translation'
  });

  return i18next;
}

module.exports = initI18n;

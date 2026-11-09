import { defineStore } from 'pinia'
import { toRaw } from 'vue'

export const dataStore = defineStore('data', {
  state: () => ({
    userSettings: {
      "Account": {
        "Name": null,
        "UserName": null,
        "Email": null
      },
      "Preferences": {
        "Language": "es-MX",
        "Theme": null,
        "ProcessViewerFile": null
      }
    }
  }),

  actions: {

    initUserSettings() {
      this.userSettings = {
        "Account": {
          "Name": null,
          "UserName": null,
          "Email": null
        },
        "Preferences": {
          "Language": "es-MX",
          "Theme": null,
          "ProcessViewerFile": null
        }
      };
    },

    setUserSettings(newSettings) {
      if (newSettings) {
        this.userSettings = newSettings;
      }
      else {
        this.initUserSettings();
      }
    },

    getLanguage() {
      return this.userSettings.Preferences.Language;
    },

    setLanguage(lang) {
      this.userSettings.Preferences.Language = lang;
    },

    getProcessViewerFile() {
      return this.userSettings.Preferences.ProcessViewerFile;
    },

    setProcessViewerFile(processViewerFile) {
      this.userSettings.Preferences.ProcessViewerFile = processViewerFile;
    }
  }
})
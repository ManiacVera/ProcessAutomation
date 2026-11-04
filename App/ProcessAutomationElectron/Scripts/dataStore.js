import { defineStore } from 'pinia'
import { toRaw } from 'vue'

export const dataStore = defineStore('data', {
  state: () => ({
    data: {
      ProcessData: {
        Number: 1,
        ProcessName: "",
        TaskList: []
      },
      UserCode: "",
      BaseDirectory: "",
      ProcessFile: ""
    },
    userSettings: {
      "Account": {
        "Name": null,
        "UserName": null,
        "Email": null
      },
      "Preferences": {
        "Language": "es-MX",
        "Theme": null
      }
    }
  }),

  actions: {

    async loadLocalData() {
      const saved = await window.electronAPI.getGlobal('data');
      if (saved) {
        this.data = saved;
      }
    },

    async saveLocalData() {
      await window.electronAPI.setGlobal('data', toRaw(this.data));
    },

    initData() {
      this.data = {
            ProcessData: {
                Number: 1,
                ProcessName: "",
                TaskList: []
            },
            UserCode: " ",
            BaseDirectory: " ",
            ProcessFile: "",
        };
    },
    
    setData(newData) {
      this.data = newData;
    },

    setProcessNumber(number) {
      this.data.ProcessData.Number = number;
    },

    setProcessName(name) {
      this.data.ProcessData.ProcessName = name;
    },
    
    setUserCode(code) {
      this.data.UserCode = code;
    },

    setUserCode(directory) {
      this.data.BaseDirectory = directory;
    },

    setProcessFile(file) {
      this.data.ProcessFile = file;
    },

    addTask(task) {
      this.data.ProcessData.TaskList.push(task)
    },

    initUserSettings() {
      this.userSettings = {
        "Account": {
          "Name": null,
          "UserName": null,
          "Email": null
        },
        "Preferences": {
          "Language": "es-MX",
          "Theme": null
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
    }
  }
})
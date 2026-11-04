<template>
  <div class="row bg-dark rounded shadow mb-2">
    <div class="col-8 d-flex align-items-center">
        <h4 class="text-white mb-0 title-shadow">Process Automation</h4>
    </div>

    <div class="col-4 d-flex align-items-center justify-content-end mt-2 mb-2">
        <label class="me-2 mb-0">{{languageLabel}}:</label>
        <select v-model="userSettings.Preferences.Language" class="w-auto me-2" @change="onChangeLanguage">
            <option v-for="item in languageList" :key="item.Value" :value="item.Value">
                {{ item.Text }}
            </option>
        </select>
    </div>
  </div>

  <div class="row bg-dark rounded shadow main-div">
    <section class="content">
      <router-view />
    </section>
  </div>
</template>

<script setup>
import { pageSetup } from "../Scripts/Pages/MasterView";
const
{
  userSettings,
  languageLabel,
  languageList,
  onChangeLanguage
} = pageSetup();
</script>
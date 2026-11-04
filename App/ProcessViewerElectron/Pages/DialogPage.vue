<template>
    <div class="main-div bg-dark rounded shadow">
        <div class="content">
            <h4 class="dialog-title">
                {{ titleLabel }}
            </h4>
            <hr class="line" />
            <p class="dialog-message">
                {{ message }}
            </p>
            <div class="dialog-buttons">
                <button v-for="(btn, index) in buttons" :key="index" :class="index === 0 ? 'btn-primary' : 'btn-secondary'" @click="selectButton(index)">
                    {{ btn }}
                </button>
            </div>
        </div>
    </div>
</template>


<style>
.main-div {
    position: absolute;
    inset: 0;

    width: 100%;
    height: 100%;

    margin: 0 !important;
    box-sizing: border-box;

    padding: 14px;
}

.content {
    width: 100%;
    height: 100%;

    display: flex;
    flex-direction: column;
}

.line {
    margin: 0 0 4px 0;
    padding: 0;
}

.dialog-title {
    margin: 0;
    padding: 0;
    color: white;

    font-size: 22px;
    font-weight: 600;

    text-shadow: 1px 1px 2px #000;
}

.dialog-message {
    margin: 2px 0 0 0;

    color: white;
    font-size: 16px;
}

.dialog-buttons {
    margin-top: auto;

    display: flex;
    justify-content: flex-end;
    gap: 10px;

    padding-bottom: 0;
}

.dialog-buttons button {
    padding: 10px 18px;

    border: none;
    border-radius: 4px;

    font-size: 16px;
    font-weight: 600;

    cursor: pointer;
}

</style>

<script setup>
    import { pageSetup } from "../Scripts/Pages/DialogPage.js";

    const
    {
        titleLabel,
		buttons,
        message,
		selectButton,
    } = pageSetup();
</script>
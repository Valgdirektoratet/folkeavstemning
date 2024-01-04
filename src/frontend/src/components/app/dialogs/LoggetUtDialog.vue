<script setup lang="ts">
import { useVModel } from '@vueuse/core';
const props = defineProps<{
    modelValue: boolean;
}>();

const emit = defineEmits(["update:modelValue"]);
const isOpen = useVModel(props, "modelValue", emit);
</script>
<template>
    <Modal v-model:modal-is-open="isOpen" :closeable="true">
        <template #title>
            <div class="flex flex-col items-center gap-2 text-2xl font-semibold text-primary-600">
                {{ $t('session.logget_ut_dialog.dialog_tittel') }}
            </div>
        </template>
        <template #description>
            {{ $t('session.logget_ut_dialog.dialog_beskrivelse') }}
        </template>
        <template #buttons>
            <StyledButton primary @click="isOpen = false">
                {{ $t('button.lukk') }}
            </StyledButton>
        </template>
    </Modal>
</template>


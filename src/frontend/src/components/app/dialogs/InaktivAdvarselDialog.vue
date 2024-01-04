<script setup lang="ts">
import { useAuthStore } from '@/stores';
import { useVModel } from '@vueuse/core';
import { ref } from 'vue';
const props = defineProps<{
    modelValue: boolean;
}>();

const emit = defineEmits(["update:modelValue"]);
const isOpen = useVModel(props, "modelValue", emit);
const authStore = useAuthStore();
const reauthenticating = ref(false);

const reauthenticate = async (count: number = 3): Promise<void> => {
    reauthenticating.value = true;
    const expiresIn = authStore.state.expiresIn;

    await authStore.getAuthenticatedUserAsync();
    await authStore.getAuthenticatedUserAsync(false);

    if(authStore.state.expiresIn === expiresIn && count > 0){
        return reauthenticate(count - 1);
    }

    reauthenticating.value = false;
    isOpen.value = false;
}

const redirect = () => window.location.href = authStore.state.logoutUrl ?? '/bff/logout';

</script>
<template>
    <Modal v-model:modal-is-open="isOpen" :closeable="false">
        <template #title>
            <div class="flex flex-col items-center gap-2 text-2xl font-semibold text-primary-600">
                {{ $t('session.inaktiv_dialog.dialog_tittel') }}
            </div>
        </template>
        <template #description>
            {{ $t('session.inaktiv_dialog.dialog_beskrivelse') }}
        </template>
        <template #buttons>
            <div class="flex flex-row-reverse gap-4">
                <StyledButton primary rounded @click="reauthenticate" :loading="reauthenticating">{{ $t('button.gi_meg_mer_tid') }}</StyledButton>
                <StyledButton outlined rounded @click="redirect">{{ $t('nav.logg_ut') }}</StyledButton>
            </div>
        </template>
    </Modal>
</template>


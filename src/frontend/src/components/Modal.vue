<script setup lang="ts">
import StyledButton from './StyledButton.vue';
import {
    Dialog,
    DialogPanel
} from '@headlessui/vue';
import { useVModel } from '@vueuse/core';

interface IModalProps {
    modalIsOpen: boolean;
    closeable: boolean;
}

const props = defineProps<IModalProps>();
const emit = defineEmits(['update:modalIsOpen']);
const modalIsOpen = useVModel(props, "modalIsOpen", emit);

const closeModal = () => {
    if(props.closeable){
        modalIsOpen.value = false;
    }
}
</script>

<template>
    <Dialog appear :open="modalIsOpen" @close="closeModal" class="relative z-10">
        <div class="fixed inset-0 bg-primary-600/50" aria-hidden="true"></div>

        <div class="fixed inset-0 flex w-screen items-center justify-center p-4 overflow-y-auto">
            <DialogPanel class="w-full max-w-sm rounded bg-white p-8 relative">
                <div class="mt-4 flex flex-col gap-4">
                    <div>
                        <slot name="title">
                            <h2>Tittel</h2>
                        </slot>
                    </div>
                    <slot name="description">
                        <p>Beskrivelse</p>
                    </slot>
                </div>

                <div class="flex flex-row-reverse justify-start gap-2 mt-5">
                    <slot name="buttons">
                        <StyledButton rounded outlined @click="closeModal">
                            Lukk
                        </StyledButton>
                    </slot>
                </div>
            </DialogPanel>
        </div>
    </Dialog>
</template>
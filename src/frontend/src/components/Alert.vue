<script setup lang="ts">
import { DriftmeldingsType } from '@/stores';
import { computed, ref } from 'vue';

const props = withDefaults(defineProps<{
    title?: string;
    description?: string;
    dismissable?: boolean;
    type?: DriftmeldingsType
}>(), {
    dismissable: true,
    type: DriftmeldingsType.INFORMATION
});

const color = computed(() => {
    if (props.type == DriftmeldingsType.INFORMATION) {
        return "text-blue-700 bg-blue-100 border-blue-300"
    }

    if (props.type == DriftmeldingsType.ERROR) {
        return "text-red-700 bg-red-100 border-red-300"
    }

    if (props.type == DriftmeldingsType.WARNING) {
        return "text-yellow-700 bg-yellow-100 border-yellow-300"
    }

    return "text-green-700 bg-green-100 border-green-300"
});

const show = ref(true);
</script>

<template>
    <div v-if="show" class="border-y" :class="color">
        <span class="container flex justify-center items-center font-medium py-1">
        <div slot="avatar" class="flex items-center">
            <span aria-hidden="true" class="material-symbols-outlined px-2">info</span>
        </div>
        <div class="text-xl font-normal max-w-full flex-initial">
            <div class="py-2">
                {{ props.title }}
                <div class="text-sm font-base" v-if="props.description">{{ props.description }}</div>
            </div>
        </div>
        <div class="flex flex-auto flex-row-reverse" >
            <div class="hover:cursor-pointer" @click="show = false" v-if="props.dismissable">
                <span aria-hidden="true" class="material-symbols-outlined scale-75">close</span>
            </div>
        </div>
    </span>
    </div>
</template>
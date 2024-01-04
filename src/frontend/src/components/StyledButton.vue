<script setup lang="ts">
import { computed } from 'vue';

interface StyledLinkProps {
    primary?: boolean,
    outlined?: boolean,
    disabled?: boolean;
    loading?: boolean;
}

const props = withDefaults(defineProps<StyledLinkProps>(), {
    primary: true,
});

const type = computed(() => {
    if(props.disabled){
        return "bg-primary-300 border-solid border-primary-400 border-2";
    }
    
    if(props.outlined){
        return "active:bg-primary-600 active:text-white bg-white text-black border-solid border-2 border-primary-600 hover:bg-primary-500/20 hover:text-primary-500";
    }
    
    if(props.primary){
        return "bg-primary-500 text-white hover:bg-primary-600 hover:text-white";
    }
});
</script>

<template>
    <button class="px-4 py-2 font-bold text-sm flex items-center focus:outline-2 outline-offset-4 outline-primary-500 rounded-full"
        :class="type"
        :disabled="props.disabled || props.loading">
        <Spinner v-if="props.loading" />
        <slot></slot>
    </button>
</template>
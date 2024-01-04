<script setup lang="ts">
import { useDriftsmeldingerStore } from '@/stores';
import { onBeforeMount } from 'vue';

const driftsmeldingerStore = useDriftsmeldingerStore();

onBeforeMount(async () => driftsmeldingerStore.getAllDriftsmeldingerAsync());
</script>

<template>
    <Alert v-for="driftsmelding in driftsmeldingerStore.state.driftsmeldinger" :title="driftsmelding.tittel"
        :description="driftsmelding.informasjon" :type="driftsmelding.type" :dismissable="false" />
</template>
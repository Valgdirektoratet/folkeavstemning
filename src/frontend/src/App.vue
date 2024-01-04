<script setup lang="ts">
import { RouterView } from 'vue-router';
import { useRouterFocusReset } from './composables/routerFocusReset';

useRouterFocusReset();
</script>

<template>
  <div class="flex flex-col h-screen justify-between">
    <div>
      <Navbar />
      <Driftsmeldinger />
      <AuthStatusChecker />
      <RouterView />
    </div>
    <Footer />
  </div>
</template>
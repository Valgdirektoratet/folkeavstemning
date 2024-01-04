<script setup lang="ts">
import { computed } from "vue";
import type { RouteLocationRaw } from "vue-router";

interface StyledLinkProps {
  to?: string | RouteLocationRaw,
  href?: string,
  outlined?: boolean,
  primary?: boolean;
}

const props = defineProps<StyledLinkProps>();

const type = computed(() => {
  if (props.outlined) {
    return "active:bg-primary-600 active:text-white bg-white text-black border-solid border-2 border-primary-600 hover:bg-primary-500/20 hover:text-primary-500";
  }

  if (props.primary) {
    return "bg-primary-500 hover:bg-primary-600 text-white";
  }
});
</script>

<template>
  <RouterLink v-if="to" :to="to" :class="type, (props.outlined || props.primary) && 'shadow-lg px-4 py-1.5'"
    class="focus:outline-2 outline-offset-4 outline-primary-500 rounded-full inline-flex items-center font-bold text-sm">
    
    <slot></slot>
  </RouterLink>
  <a v-else-if="href" :href="href" :class="type, (props.outlined || props.primary) && 'shadow-lg px-4 py-1.5'"
    class="focus:outline-2 outline-offset-4 outline-primary-500 rounded-full inline-flex items-center font-bold text-sm">
    <slot></slot>
  </a>
  <pre class="text-red-500" v-else>Specify to or href</pre>
</template >

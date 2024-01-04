<script setup lang="ts">
import { ref } from 'vue';

const showMore = ref(false)
defineProps<{ header?: string, body?: string }>()

</script>
<template>
  <div class="text-left">
    <p>
      {{ header }}
    </p>
    <Transition enter-from-class="max-h-0" enter-to-class="max-h-screen" leave-to-class="max-h-0"
      enter-active-class="transition-all duration-500" leave-active-class="transition-all duration-500">
      <div v-if="showMore" class="overflow-hidden">
        <p class="border-l-2 mb-1 mt-3 pl-3" aria-live="polite">
          {{ body }}
        </p>
      </div>
    </Transition>
    <button @click="showMore = !showMore"
      class="text-primary-500 text-left hover:text-black flex items-center focus:ring-2 p-1 ring-primary-500 outline-none"
      :aria-expanded="showMore">
      <span class="underline">{{ showMore ? $t('label.vis_mindre') : $t('label.vis_mer') }}</span>
      <span :aria-label="showMore ? $t('aria.vis_mindre_informasjon'): $t('aria.vis_mer_informasjon')" class="material-symbols-outlined">{{ showMore ? 'expand_less' : 'expand_more'
      }}</span>
    </button>
  </div>
</template>
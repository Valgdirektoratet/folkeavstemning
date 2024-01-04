<script setup lang="ts">
import Container from './Container.vue';
</script>

<template>
  <footer class="bg-primary-200 w-full py-4 pb-6 px-2 sm:px-0 text-sm place-self-end">
    <section class="px-4 sm:px-0">
      <Container>
        <div class="grow flex flex-col gap-2">
          <h3 class="font-bold text-xl pt-2 text-primary-500">{{ $t('footer.tittel')}}</h3>
          <p>{{ $t("footer.hjelpetekst") }}</p>
          <div class="flex-col sm:flex-row flex justify-center md:justify-start gap-2 items-center">
            <a href="mailto:folkeavstemning@valgdirektoratet.no" class=" focus:ring-2 py-1 ring-primary-500 outline-none">
              {{ $t("footer.epost") }}<span class="underline">folkeavstemning@valgdirektoratet.no</span></a>
            <p class="hidden sm:block font-bold">|</p>
            <p>{{ $t("footer.telefon", ['33 50 79 06']) }}</p>
          </div>
          <div class="flex gap-2 items-center">
            <a href="valg.no/folkeavstemning" class="underline hover:text-primary-500 focus:ring-2 py-1 ring-primary-500 outline-none">{{
              $t("footer.om_folkeavstemninger") }}</a> | 
              <router-link to="/erklaring-om-informasjonskapsler" target="_blank" class="underline hover:text-primary-500 focus:ring-2 py-1 ring-primary-500 outline-none"> {{ $t('footer.informasjonskapsler') }} </router-link>
               | <a href="https://www.valg.no/annet/valgdirektoratets-personvernerklaring/" target="_blank" class="underline hover:text-primary-500 focus:ring-2 py-1 ring-primary-500 outline-none">{{
              $t("footer.linkToPersonvernserklæring") }}</a>
          </div>
        </div>
      </Container>
    </section>
  </footer>
</template>
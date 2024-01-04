<script setup lang="ts"></script>
<template>
    <Container>
        <div class="flex gap-2 flex-col">
            <h1 class="text-2xl sm:text-3xl text-primary-500 mt-12">{{ $t('cookie_erklæring_view.tittel') }}</h1>
            <p class="text-lg">{{ $t('cookie_erklæring_view.undertittel') }}</p>
            <p>{{ $t('cookie_erklæring_view.informasjon1') }}</p>
            <ul class="list-square list-inside my-4">
                <li>{{ $t('cookie_erklæring_view.cookie_usage.usecase1') }}</li>
                <li>{{ $t('cookie_erklæring_view.cookie_usage.usecase2') }}</li>
            </ul>
            <p>{{ $t('cookie_erklæring_view.informasjon2') }}</p>
            <p>
                {{ $t('cookie_erklæring_view.informasjon3') }}
                <a href="https://lovdata.no/dokument/NL/lov/2003-07-04-83/KAPITTEL_2#%C2%A72-7b" class="underline text-primary-500">{{ $t('cookie_erklæring_view.informasjon3_link') }}</a>
            </p>
        
            <h2 class="text-lg text-primary-500 mt-4">{{ $t('cookie_erklæring_view.tittel2') }}</h2>
        
            <div>
                <p class="uppercase">{{ $t('cookie_erklæring_view.cookie1.tittel') }}</p>
                <p>{{ $t('cookie_erklæring_view.cookie1.beskrivelse') }}</p>
            </div>
        </div>
    </Container>
</template>
<script setup lang="ts">
import { useAuthStore } from '@/stores';
import { watch } from 'vue';
import { ref } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const authStore = useAuthStore();
const showInaktivWarningDialog = ref(false);
const showLoggetUtDialog = ref(false);
const route = useRoute();
const router = useRouter();

if (authStore.state.isLoggedIn) {
    setInterval(async () => {

        const fiveMinutesBeforeExpiry = new Date(Date.now() + (5 * 1_000 * 60)) >= authStore.state.expiresIn;
        const fifteenSecondsBeforeExpiry = new Date(Date.now() + ((15 / 60) * 1_000 * 60)) >= authStore.state.expiresIn;
        const isExpired = new Date() > authStore.state.expiresIn;
        
        // session i BFF vil være ugyldig
        if (isExpired) {
            window.location.href = `${window.location.href}?loggUtInaktiv=true`
        }
        
        // 15 sekunder før fordi hvis vi sender på akkurat dato vil ikke session være gyldig i duende
        if (fifteenSecondsBeforeExpiry) {
            if (authStore.state.logoutUrl !== undefined) {
                window.location.href = `${authStore.state.logoutUrl}&returnUrl=${encodeURIComponent('/?loggUtInaktiv=true')}`;
            } else {
                window.location.href = `/bff/logout?returnUrl=${encodeURIComponent('/?loggUtInaktiv=true')}`;
            }
        }
        
        if(fiveMinutesBeforeExpiry){
            await authStore.getAuthenticatedUserAsync(false);

            if(showInaktivWarningDialog.value === false) showInaktivWarningDialog.value = true;
        }

    }, 10_000);
}

watch(route, async () => {
    if (route.query.loggUtInaktiv === "true") {
        await router.replace({
            query: {}
        });
        showLoggetUtDialog.value = true;
    } else {
        showLoggetUtDialog.value = false;
    }
});

</script>
<template>
    <InaktivAdvarselDialog v-model="showInaktivWarningDialog" />
    <LoggetUtDialog v-model="showLoggetUtDialog" />
</template>
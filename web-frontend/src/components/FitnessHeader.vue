<template>
  <header class="ft-header">
    <div class="ft-header-inner">
      <div class="ft-brand">
        <v-icon color="primary" size="26">mdi-trophy-outline</v-icon>
        <div>
          <div class="ft-brand-title">GIBZ Fitnesstest</div>
          <div class="ft-brand-sub">Gewerblich-industrielles Bildungszentrum Bern</div>
        </div>
      </div>
      <nav class="ft-nav">
        <button
          :class="['ft-nav-btn', isLeaderboard && 'ft-nav-btn--active']"
          @click="router.push('/fitnesstest')"
        >
          <v-icon size="16">mdi-trophy</v-icon>
          Globale Bestenliste
        </button>
        <button
          :class="['ft-nav-btn', isKlassen && 'ft-nav-btn--active']"
          @click="router.push('/fitnesstest/klassen')"
        >
          <v-icon size="16">mdi-account-group</v-icon>
          Klassen-Verwaltung
        </button>
      </nav>
    </div>
  </header>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();

const isLeaderboard = computed(() => route.path === '/fitnesstest');
const isKlassen = computed(() => route.path.startsWith('/fitnesstest/klassen'));
</script>

<style scoped>
.ft-header {
  background: #fff;
  border-bottom: 1px solid #E2E8F0;
  padding: 0 32px;
  height: 60px;
  display: flex;
  align-items: center;
}

.ft-header-inner {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.ft-brand {
  display: flex;
  align-items: center;
  gap: 10px;
}

.ft-brand-title {
  font-size: 1rem;
  font-weight: 700;
  color: #0F172A;
  line-height: 1.2;
}

.ft-brand-sub {
  font-size: 0.72rem;
  color: #64748B;
}

.ft-nav {
  display: flex;
  gap: 8px;
}

.ft-nav-btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 7px 16px;
  border-radius: 6px;
  font-size: 0.875rem;
  font-weight: 500;
  cursor: pointer;
  border: 1px solid transparent;
  background: transparent;
  color: #64748B;
  transition: all 0.15s;
}

.ft-nav-btn:hover {
  background: #F1F5F9;
  color: #1E293B;
}

.ft-nav-btn--active {
  background: #2563EB;
  color: #fff;
  border-color: #2563EB;
}

.ft-nav-btn--active:hover {
  background: #1D4ED8;
}
</style>

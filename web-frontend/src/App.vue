<script setup lang="ts">
</script>

<template>
  <div id="app">
    <header class="app-header">
      <div class="header-content">
        <h1 class="app-title">
          <router-link to="/">GIBZ Fitnesstest</router-link>
        </h1>
        <nav class="main-nav">
          <router-link to="/" class="nav-link">Klassen</router-link>
          <router-link to="/leaderboard" class="nav-link">Bestenliste</router-link>
        </nav>
      </div>
    </header>
    <main class="app-main">
      <router-view />
    </main>
  </div>
</template>

<style scoped>
.app-header {
  background-color: #1a365d;
  color: white;
  padding: 0 1.5rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.header-content {
  max-width: 1400px;
  margin: 0 auto;
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 60px;
}

.app-title {
  font-size: 1.25rem;
  font-weight: 700;
  margin: 0;
}

.app-title a {
  color: white;
  text-decoration: none;
}

.main-nav {
  display: flex;
  gap: 1rem;
}

.nav-link {
  color: rgba(255, 255, 255, 0.85);
  text-decoration: none;
  padding: 0.5rem 1rem;
  border-radius: 6px;
  font-weight: 500;
  transition: all 0.2s;
}

.nav-link:hover,
.nav-link.router-link-active {
  color: white;
  background-color: rgba(255, 255, 255, 0.15);
}

.app-main {
  max-width: 1400px;
  margin: 0 auto;
  padding: 1.5rem;
}
</style>

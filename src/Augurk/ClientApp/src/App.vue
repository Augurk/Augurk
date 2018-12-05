<template>
  <v-app>
    <v-toolbar app :dark="darkmode" extended flat color="primary" :extension-height="onHomePage ? 250 : 1">
      <v-toolbar-side-icon v-show="!onHomePage" to="/">
        <v-img :src="require('./assets/logo.png')"></v-img>
      </v-toolbar-side-icon>
      <v-toolbar-title v-show="!onHomePage">
        {{instanceName}}
      </v-toolbar-title>
      <v-breadcrumbs :items="items"></v-breadcrumbs>
      <v-spacer></v-spacer>
      <v-btn icon @click="onHomePage = !onHomePage">
        <v-icon>highlight</v-icon>
      </v-btn>
      <v-btn icon>
        <v-icon>settings</v-icon>
      </v-btn>
      <v-layout slot="extension" row v-show="onHomePage">
        <v-flex xs6>
          <v-img :src="require('./assets/logo.png')" contain position="center right" height="200"></v-img>
        </v-flex>
        <v-flex xs6>
          <v-layout column justify-space-between fill-height>
            <v-spacer></v-spacer>
            <v-toolbar-title class="headline">
              <span>{{instanceName}}</span>
            </v-toolbar-title>
            <v-spacer></v-spacer>
         </v-layout>
        </v-flex>
      </v-layout>
    </v-toolbar>

    <v-content>
      <router-view></router-view>
    </v-content>

    <v-footer app>
      <v-layout justify-center row wrap>
        <v-flex primary lighten-2 py-3 text-xs-center xs12>
          &copy;2018 — <strong>Augurk {{augurkVersion}}</strong>
        </v-flex>
      </v-layout>
    </v-footer>
  </v-app>
</template>

<script>
export default {
  name: 'App',
  components: {
  },
  data() {
    return {
      darkmode: false,
      items: [],
    };
  },
  computed: {
    instanceName() {
      return this.$store.state.customization ? this.$store.state.customization.instanceName : '';
    },
    augurkVersion() {
      return this.$store.state.augurkVersion;
    },
    onHomePage() {
      return this.$route.name === 'home';
    },
  },
  mounted() {
    this.$store.dispatch('loadAugurkVersion');
    this.$store.dispatch('loadCustomization');
  },
};
</script>

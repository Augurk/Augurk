<template>
  <v-app>
    <v-toolbar app :dark="darkmode" extended flat color="primary" extension-height="250">
      <v-spacer></v-spacer>
      <v-btn icon @click="darkmode = !darkmode">
        <v-icon>highlight</v-icon>
      </v-btn>
      <v-btn icon>
        <v-icon>settings</v-icon>
      </v-btn>
      <v-layout slot="extension" row>
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
    </v-content>

    <v-footer app>
      <v-layout justify-center row wrap>
        <v-flex primary lighten-2 py-3 text-xs-center xs12>
          &copy;2018 — <strong>Augurk</strong>
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
    };
  },
  computed: {
    instanceName() {
      return this.$store.state.customization.instanceName;
    },
  },
  mounted() {
    this.$store.dispatch('loadCustomization');
  },
};
</script>

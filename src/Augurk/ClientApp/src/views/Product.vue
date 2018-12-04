<template>
    <v-container>
        <v-card>
            <v-card-title primary-title>
                <div>
                    <div class="headline">{{product.name}}</div>
                </div>
            </v-card-title>
            <v-slide-y-transition>
                <v-card-text v-show="showDescription">
                    <vue-markdown :breaks="false">{{product.description}}</vue-markdown>
                </v-card-text>
            </v-slide-y-transition>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn icon @click="showDescription = !showDescription">
                    <v-icon>{{ showDescription ? 'keyboard_arrow_down' : 'keyboard_arrow_up' }}</v-icon>
                </v-btn>
            </v-card-actions>
        </v-card>
    </v-container>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import VueMarkdown from 'vue-markdown';

@Component({
  components: {
      VueMarkdown
  },
  data() {
    return {
        showDescription: true
    }
  },
  computed: {
      product() {
          const product = this.$store.state.products.find((p) => p.name === this.$route.params.name);
          return product;
      },
  },
  mounted() {
      this.$store.dispatch('ensureProductLoaded', this.$route.params.name);
  },
})
export default class Product extends Vue {}
</script>

<style scoped>

</style>
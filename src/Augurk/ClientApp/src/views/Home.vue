<template>
  <div>
    <v-container>
      <v-layout row>
        <v-text-field solo>
          <v-btn icon slot="append">
            <v-icon>search</v-icon>
          </v-btn>
        </v-text-field>
      </v-layout>
    </v-container>
    <v-container grid-list-md>
      <v-layout row wrap>
        <v-flex v-for="product in products" :key="product" xs12 md3>
          <Product :product="product" />
        </v-flex>
      </v-layout>
    </v-container>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import Product from '../components/Product.vue';

@Component({
  components: {
    Product,
  },
  computed: {
    products() {
      return this.$store.state.products;
    },
  },
  mounted() {
    this.$store.dispatch('loadProducts');
  },
})
export default class Home extends Vue {}
</script>

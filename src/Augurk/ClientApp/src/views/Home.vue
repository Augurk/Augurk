<template>
  <div>
    <v-container pb-0>
      <v-layout row>
        <v-text-field solo placeholder="Search">
          <v-btn icon slot="append">
            <v-icon>search</v-icon>
          </v-btn>
        </v-text-field>
      </v-layout>
    </v-container>
    <v-container pt-0 grid-list-sm>
      <v-layout row wrap>
        <v-flex v-for="product in products" :key="product.name" xs12 md3>
          <ProductCard :product="product" />
        </v-flex>
      </v-layout>
    </v-container>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import ProductCard from '../components/ProductCard.vue';
import products from '../store/products';

@Component({
  components: {
    ProductCard,
  },
})
export default class Home extends Vue {
  private get products() {
    return products.state.products;
  }

  private mounted() {
    products.dispatchLoadProducts();
  }
}
</script>

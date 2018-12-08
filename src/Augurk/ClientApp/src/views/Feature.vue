<template>
    <v-container fluid>
        <v-layout row wrap>
            <v-flex xs3>
                <ProductNavigation :product="product" />
            </v-flex>
            <v-flex xs9>
                <h1>Hello from feature {{feature}}</h1>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import { ProductsModule } from '../store/products';
import ProductNavigation from '../components/ProductNavigation.vue';

@Component({
    components: {
        ProductNavigation,
    },
})
export default class Feature extends Vue {
    private showDescription = true;

    private get product() {
        return ProductsModule.selectedProduct;
    }

    private get feature() {
        return this.$route.params.featureName;
    }

    private mounted() {
        ProductsModule.loadProductDetails(this.$route.params.productName);
    }
}
</script>

<style scoped>

</style>
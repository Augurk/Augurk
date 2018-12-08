<template>
    <div>
        <h1>Hello Feature {{feature}} from product {{product.name}}</h1>
    </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import { ProductsModule } from '../store/products';

@Component
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
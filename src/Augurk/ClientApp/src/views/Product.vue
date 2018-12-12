<template>
    <v-container fluid>
        <v-layout row wrap>
            <v-flex xs3>
                <ProductNavigation :product="product" />
            </v-flex>
            <v-flex xs9>
                <v-card flat class="md6">
                    <v-card-title primary-title>
                        <div>
                            <div class="headline">{{name}}</div>
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
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
import { Component, Vue, Prop } from 'vue-property-decorator';
import { ProductsModule } from '../store/products';
import ProductNavigation from '../components/ProductNavigation.vue';

@Component({
    components: {
        ProductNavigation,
    },
})
export default class Product extends Vue {
    @Prop(String) public name!: string;

    private showDescription = true;

    private get product() {
        return ProductsModule.selectedProduct;
    }

    private created() {
        ProductsModule.loadProductDetails(this.name);
    }
}
</script>

<style scoped>

</style>
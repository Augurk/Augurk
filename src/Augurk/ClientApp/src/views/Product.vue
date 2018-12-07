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
import products from '../store/products';

@Component
export default class Product extends Vue {
    private showDescription = true;

    private get product() {
        return products.getProductByName(this.$route.params.name);
    }

    private mounted() {
        products.dispatchEnsureProductLoaded(this.$route.params.name);
    }
}
</script>

<style scoped>

</style>
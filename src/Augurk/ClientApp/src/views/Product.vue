<template>
    <v-container fluid>
        <v-layout row wrap>
            <v-flex xs3>
                <v-card class="d-inline-block elevation-12" width="350">
                    <v-navigation-drawer floating stateless value="true" width="350">
                        <v-list expand>
                            <v-list-tile>
                                <v-list-tile-title>{{ product.name }}</v-list-tile-title>
                            </v-list-tile>
                            <v-list-group no-action v-for="group in product.groups" :key="group.name" value="true">
                                <v-list-tile slot="activator">
                                    <v-list-tile-title>{{ group.name }}</v-list-tile-title>
                                </v-list-tile>
                                <v-list-tile v-for="feature in group.features" :key="feature.name">
                                    <v-list-tile-title v-text="feature.title"></v-list-tile-title>
                                </v-list-tile>
                            </v-list-group>
                        </v-list>
                    </v-navigation-drawer>
                </v-card>
            </v-flex>
            <v-flex xs9>
                <v-card class="md6">
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
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import { ProductsModule } from '../store/products';

@Component
export default class Product extends Vue {
    private showDescription = true;

    private get product() {
        return ProductsModule.selectedProduct;
    }

    private mounted() {
        ProductsModule.loadProductDetails(this.$route.params.name);
    }
}
</script>

<style scoped>

</style>
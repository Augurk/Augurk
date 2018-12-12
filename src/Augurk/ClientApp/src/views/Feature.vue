<template>
    <v-container fluid>
        <v-layout row wrap>
            <v-flex xs3>
                <ProductNavigation :product="product" />
            </v-flex>
            <v-flex xs9>
                <h1>{{feature.title}}</h1>
                <vue-markdown :breaks="false" :source="feature.description"></vue-markdown>
                <v-tabs>
                    <v-tab :key="scenarios">Scenarios</v-tab>
                    <v-tab-item :key="scenarios">
                        <v-card flat>
                            Here come the scenarios
                        </v-card>
                    </v-tab-item>
                    <v-tab :key="dependencies">Dependencies</v-tab>
                    <v-tab-item :key="dependencies">
                        <v-card flat>
                            Here come the dependencies
                        </v-card>
                    </v-tab-item>
                    <v-tab :key="history">History</v-tab>
                    <v-tab-item :key="history">
                        <v-card flat>
                            Here comes the history
                        </v-card>
                    </v-tab-item>
                </v-tabs>
            </v-flex>
        </v-layout>
    </v-container>
</template>

<script lang="ts">
import { Component, Vue, Prop, Watch } from 'vue-property-decorator';
import { ProductsModule } from '../store/products';
import { FeaturesModule } from '../store/features';
import ProductNavigation from '../components/ProductNavigation.vue';

@Component({
    components: {
        ProductNavigation,
    },
})
export default class Feature extends Vue {
    @Prop(String) public productName!: string;
    @Prop(String) public groupName!: string;
    @Prop(String) public featureName!: string;
    private showDescription = true;

    private get product() {
        return ProductsModule.selectedProduct;
    }

    private get feature() {
        return FeaturesModule.selectedFeature;
    }

    private created() {
        this.fetchData();
    }

    @Watch('$route')
    private onRouteUpdate() {
        this.fetchData();
    }

    private fetchData() {
        ProductsModule.loadProductDetails(this.productName);
        FeaturesModule.loadFeature({
            productName: this.productName,
            groupName: this.groupName,
            featureTitle: this.featureName,
        });
    }
}
</script>

<style scoped>

</style>
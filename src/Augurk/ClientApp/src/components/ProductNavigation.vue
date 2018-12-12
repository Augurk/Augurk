<template>
    <v-card class="d-inline-block elevation-12" width="350">
        <v-navigation-drawer floating stateless value="true" width="350">
            <v-list expand>
                <v-list-tile :to="`/products/${product.name}`">
                    <v-list-tile-title>{{ product.name }}</v-list-tile-title>
                </v-list-tile>
                <v-list-group no-action v-for="group in product.groups" :key="group.name" value="true">
                    <v-list-tile slot="activator">
                        <v-list-tile-title>{{ group.name }}</v-list-tile-title>
                    </v-list-tile>
                    <template v-for="feature in group.features">
                        <v-list-tile v-if="!feature.childFeatures" :key="feature.title" :to="`/products/${product.name}/groups/${group.name}/features/${feature.title}`">
                            <v-list-tile-title v-text="feature.title"></v-list-tile-title>
                        </v-list-tile>
                        <v-list-group v-else :key="feature.title" value="true">
                            <v-list-tile slot="activator" :to="`/products/${product.name}/groups/${group.name}/features/${feature.title}`">
                                <v-list-tile-title>{{ feature.title }}</v-list-tile-title>
                            </v-list-tile>
                            <v-list-tile v-for="childFeature in feature.childFeatures" :key="childFeature.title"
                                         :to="`/products/${product.name}/groups/${group.name}/features/${childFeature.title}`">
                                <v-list-tile-title>{{ childFeature.title }}</v-list-tile-title>
                            </v-list-tile>
                        </v-list-group>
                    </template>
                </v-list-group>
            </v-list>
        </v-navigation-drawer>
    </v-card>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

@Component({
  components: {
  },
  props: ['product'],
})
export default class ProductNavigation extends Vue {}
</script>

<style scoped>
</style>
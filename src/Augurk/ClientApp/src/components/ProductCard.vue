<template>
    <v-card ripple :to="product.name" slot-scope="{}" height="250px" class="align-center">
        <v-card-title primary-title>
            <div class="overflow-hidden">
                <h1 v-if="!product.description">{{product.name}}</h1>
                <vue-markdown v-if="product.description" class="description">{{product.description}}</vue-markdown>
            </div>
        </v-card-title>
    </v-card>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import VueMarkdown from 'vue-markdown';

@Component({
  components: {
      VueMarkdown,
  },
  props: ['product'],
})
export default class ProductCard extends Vue {}
</script>

<style scoped>
.overflow-hidden {
    height: 200px;
    overflow: hidden;
}
</style>
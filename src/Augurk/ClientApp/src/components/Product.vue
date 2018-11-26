<template>
    <v-card slot-scope="{}" height="200" class="align-center">
        <v-card-title primary-title>
            <div>
                <div class="headline">{{product}}</div>
                <span>Lorum ipsum</span>
            </div>
        </v-card-title>
    </v-card>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

@Component({
  components: {
  },
  props: ['product'],
})
export default class Product extends Vue {}
</script>

<style scoped>

</style>
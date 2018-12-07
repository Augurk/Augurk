import Vue from 'vue';
import Vuex, { Store } from 'vuex';
import { getStoreBuilder } from 'vuex-typex';
import { GlobalState } from './global';
import { ProductsState } from './products';

import './global';
import './products';

export interface RootState {
    global: GlobalState;
    products: ProductsState;
}

Vue.use(Vuex);

const store: Store<RootState> = getStoreBuilder<RootState>().vuexStore();
export default store;

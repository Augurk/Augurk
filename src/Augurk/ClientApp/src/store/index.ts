import Vue from 'vue';
import Vuex from 'vuex';
import { GlobalState } from './global';
import { ProductsState } from './products';

Vue.use(Vuex);

export interface RootState {
    global: GlobalState;
    products: ProductsState;
}

export default new Vuex.Store<RootState>({
    strict: process.env !== 'production',
});

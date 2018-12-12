import Vue from 'vue';
import Vuex from 'vuex';
import { GlobalState } from './global';
import { ProductsState } from './products';
import { FeaturesState } from './features';

Vue.use(Vuex);

export interface RootState {
    global: GlobalState;
    products: ProductsState;
    features: FeaturesState;
}

export default new Vuex.Store<RootState>({
    strict: process.env !== 'production',
});

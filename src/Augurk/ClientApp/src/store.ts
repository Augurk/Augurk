import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex);

export interface RootState {
  augurkVersion: string;
  customization?: {
    instanceName: string;
  };
  products: Array<{
    name: string;
    description: string;
  }>;
}

export default new Vuex.Store<RootState>({
  strict: process.env.NODE_ENV !== 'production',
  state: {
    augurkVersion: '',
    customization: undefined,
    products: [],
  },
  mutations: {
    setCustomization(state, customization) {
      state.customization = customization;
    },
    setProducts(state, products) {
      state.products = products;
    },
    setAugurkVersion(state, version) {
      state.augurkVersion = version;
    },
    addProduct(state, product) {
      state.products.push(product);
    },
  },
  actions: {
    async loadCustomization(context) {
      const result = await fetch('/api/v2/customization');
      const customization = await result.json();
      context.commit('setCustomization', customization);
    },
    async loadProducts(context) {
      const result = await fetch('/api/v3/products');
      const products = await result.json();
      context.commit('setProducts', products);
    },
    async loadAugurkVersion(context) {
      const result = await fetch('/api/version');
      const version = await result.text();
      context.commit('setAugurkVersion', version);
    },
    async ensureProductLoaded(context, productName) {
      let product = context.state.products.find((p) => p.name === productName);
      if (!product) {
        const result = await fetch('/api/v3/products/' + productName);
        product = await result.json();
        context.commit('addProduct', product);
      }
    },
  },
});

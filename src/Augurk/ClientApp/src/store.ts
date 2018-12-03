import Vue from 'vue';
import Vuex from 'vuex';

Vue.use(Vuex);

export default new Vuex.Store({
  strict: process.env.NODE_ENV !== 'production',
  state: {
    customization: {},
    products: [],
  },
  mutations: {
    setCustomization(state, customization) {
      state.customization = customization;
    },
    setProducts(state, products) {
      state.products = products;
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
  },
});

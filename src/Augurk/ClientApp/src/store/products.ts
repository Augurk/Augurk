import { getStoreBuilder, BareActionContext } from 'vuex-typex';
import { RootState } from './index';

export interface ProductsState {
    products: Product[];
}

export interface Product {
    name: string;
    description: string;
}

const initialProductsState: ProductsState = {
    products: [],
};

const b = getStoreBuilder<RootState>().module('products', initialProductsState);

// Getters
const productByNameGetter = b.read((state) => (productName: string) => {
    return state.products.find((p) => p.name === productName);
}, 'getProductByName');

// Mutations
function setProducts(state: ProductsState, payload: Product[]) {
    state.products = payload;
}

function addProduct(state: ProductsState, payload: Product) {
    state.products.push(payload);
}

// Actions
async function loadProducts() {
    const result = await fetch('/api/v3/products');
    const productList = await result.json();
    products.commitSetProducts(productList);
}

async function ensureProductLoaded(context: BareActionContext<ProductsState, RootState>, productName: string) {
    let product = context.state.products.find((p) => p.name === productName);
    if (!product) {
        const result = await fetch('/api/v3/products/' + productName);
        product = await result.json();
        if (product) {
            products.commitAddProduct(product);
        }
    }
}
// state
const stateGetter = b.state();

// exported "global" module interface
const products = {
    // state
    get state() { return stateGetter(); },

    // getters
    getProductByName(name: string) {
        return productByNameGetter()(name);
    },

    // mutations
    commitSetProducts: b.commit(setProducts),
    commitAddProduct: b.commit(addProduct),

    // actions
    dispatchLoadProducts: b.dispatch(loadProducts),
    dispatchEnsureProductLoaded: b.dispatch(ensureProductLoaded),
};

export default products;

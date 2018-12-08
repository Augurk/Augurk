import {Module, VuexModule, Mutation, Action, getModule} from 'vuex-module-decorators';
import store from '@/store';

export interface ProductsState {
    productList: Product[];
    selectedProduct: ProductDetails | null;
}

export interface Product {
    name: string;
    description: string;
}

export interface ProductDetails extends Product {
    groups: Group[];
}

export interface Group {
    name: string;
}

@Module({ dynamic: true, store, name: 'products'})
class Products extends VuexModule implements ProductsState {
    public productList: Product[] = [];
    public selectedProduct: ProductDetails | null = null;

    public get productByName() {
        return (productName: string) => this.productList.find((p) => p.name === productName);
    }

    @Mutation
    public setProducts(payload: Product[]) {
        this.productList = payload;
    }

    @Mutation
    public setSelectedPrdocut(payload: ProductDetails) {
        this.selectedProduct = payload;
    }

    @Action
    public async loadProducts() {
        const result = await fetch('/api/v3/products');
        const productList = await result.json();
        this.context.commit('setProducts', productList);
    }

    @Action
    public async loadProductDetails(productName: string) {
        const result = await fetch('/api/v3/products/' + productName);
        const productDetails = await result.json();
        this.context.commit('setSelectedPrdocut', productDetails);
    }
}

export const ProductsModule = getModule(Products);

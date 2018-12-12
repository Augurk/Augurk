import {Module, VuexModule, Mutation, Action, getModule} from 'vuex-module-decorators';
import store from '@/store';

export interface FeaturesState {
    selectedFeature: Feature | null;
}

export interface Feature {
    title: string;
    description: string;
    tags: string[];
}

@Module({ dynamic: true, store, name: 'features'})
class Features extends VuexModule implements FeaturesState {
    public selectedFeature: Feature | null = null;


    @Mutation
    public setSelectedFeature(payload: Feature) {
        this.selectedFeature = payload;
    }

    @Action
    public async loadFeature(payload: {productName: string, groupName: string, featureTitle: string}) {
        const result = await fetch(`/api/v2/products/${payload.productName}/groups/${payload.groupName}` +
                                   `/features/${payload.featureTitle}/versions/2.8.0`);
        const featureDetails = await result.json();
        this.context.commit('setSelectedFeature', featureDetails);
    }
}

export const FeaturesModule = getModule(Features);

import { getStoreBuilder } from 'vuex-typex';
import { RootState } from './index';

export interface GlobalState {
    augurkVersion: string;
    customization?: Customization;
}

export interface Customization {
    instanceName: string;
}

const initialIGlobalState: GlobalState = {
    augurkVersion: '',
    customization: undefined,
};

// Mutations
function setCustomization(state: GlobalState, payload: Customization) {
    state.customization = payload;
}

function setAugurkVersion(state: GlobalState, payload: string) {
    state.augurkVersion = payload;
}

// Actions
async function initialize() {
    // Get the version of Augurk from the backend
    const versionResult = await fetch('/api/version');
    const version = await versionResult.text();
    global.commitSetAugurkVersion(version);

    // Next load any customizations that have been set
    const customizationResult = await fetch('/api/v2/customization');
    const customization = await customizationResult.json();
    global.commitSetCustomization(customization);
}

const p = getStoreBuilder<RootState>().module('global', initialIGlobalState);

// state
const stateGetter = p.state();

// exported "global" module interface
const global = {
    // state
    get state() { return stateGetter(); },

    // mutations
    commitSetCustomization: p.commit(setCustomization),
    commitSetAugurkVersion: p.commit(setAugurkVersion),

    // actions
    dispatchInitialize: p.dispatch(initialize),
};

export default global;

<template>
    <div class="group-item">

        <v-expansion-panels :value="0">
            <v-expansion-panel>
                <v-expansion-panel-header>{{title}}</v-expansion-panel-header>
                <v-expansion-panel-content>
                    <slot></slot>
                </v-expansion-panel-content>
            </v-expansion-panel>
        </v-expansion-panels>
    </div>
</template>

<script>
    export default {
        props: ['title'],
        data () {
            return {
            }
        },
        methods: {

        }

    };
</script>

<style>
    .group-item.level-1 {
        margin-top: 1rem;
    }

    .group-item.level-2 {
        margin-bottom: .5rem;
    }

    .group-item.level-2:last-child {
        margin-bottom: 0;
    }

</style>
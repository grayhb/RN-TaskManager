<template>
    <v-app id="inspire">

        <v-navigation-drawer v-model="drawer"
                             :clipped="$vuetify.breakpoint.lgAndUp"
                             app>


            <v-list dense>

                <v-divider></v-divider>

                <v-subheader>Справочники</v-subheader>

                <template v-for="item in routes.filter(e => e.panel === 'Справочники')">

                    <v-list-item :key="item.label"
                                 link
                                 :to="item.path">
                        <v-list-item-action>
                            <v-icon>{{ item.icon }}</v-icon>
                        </v-list-item-action>
                        <v-list-item-content>
                            <v-list-item-title>
                                {{ item.label }}
                            </v-list-item-title>
                        </v-list-item-content>
                    </v-list-item>

                </template>
            </v-list>

        </v-navigation-drawer>

        <v-app-bar :clipped-left="$vuetify.breakpoint.lgAndUp"
                   app
                   color="bg-rn darken-3">

            <v-app-bar-nav-icon @click.stop="drawer = !drawer"></v-app-bar-nav-icon>

            <v-toolbar-title style="width: 300px"
                             class="ml-0 pl-4">
                <span class="hidden-sm-and-down">{{ title }}</span>
            </v-toolbar-title>

            <v-spacer></v-spacer>

            <v-btn v-if="isAdmin"
                   icon
                   href="./admin"
                   v-on="on">
                <v-icon>mdi-cogs</v-icon>
            </v-btn>

        </v-app-bar>

        <v-main>
            <v-container fluid class="page-body">
                <v-row>
                    <router-view></router-view>
                </v-row>
            </v-container>
        </v-main>

    </v-app>
</template>

<script>
    export default {
        created: function () {
            //this.isAdmin = isAdmin === true;
        },
        computed: {
            routes() {
                return this.$router.options.routes.filter(e => e.display);
            },
        },
        data: () => ({
            title: 'Управление задачами',
            // потом исправить
            isAdmin: true,
            drawer: null,
        }),
        methods: {

        }
    }
</script>

<style scoped>
    .page-body {
        padding: 1rem 1.5rem;
    }
</style>
/// <binding AfterBuild='default' />

module.exports = function (grunt) {
    'use strict';

    const sass = require('node-sass');

    // Project configuration.
    grunt.initConfig({
        
        // Sass
        sass: {
            options: {
                implementation: sass,
                sourceMap: false, // Create source map
                outputStyle: 'compressed' // Minify output
            },
            dist: {
                files: [
                    {
                        expand: true, // Recursive
                        cwd: "Styles", // The startup directory
                        src: ["main.scss"], // Source files
                        dest: "wwwroot/css", // Destination
                        ext: ".min.css" // File extension
                    }
                ]
            }
        }
    });

    // Load the plugin
    grunt.loadNpmTasks('grunt-sass');

    // Default task(s).
    grunt.registerTask('default', ['sass']);
};
import * as gulp from 'gulp';
import transpile from './transpile';
import processMarkup from './process-markup';
import processCSS from './process-css';
import {build} from 'aurelia-cli';
import * as project from '../aurelia.json';
import sp from './sp'; // Our custom task
import faFonts from './fa-fonts'; // Our custom task

export default gulp.series(
  readProjectConfiguration,
  gulp.parallel(
    transpile,
    processMarkup,
    processCSS
    ,faFonts // Our custom task
  ),
  writeBundles,
  sp
);

function readProjectConfiguration() {
  return build.src(project);
}

function writeBundles() {
  return build.dest();
}

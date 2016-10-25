import * as gulp from 'gulp';
import * as changed from 'gulp-changed';
import * as project from '../aurelia.json';

export default function sp() {
  return  gulp.src('scripts/**/*', {base:"."})
        .pipe(gulp.dest('../spaurelia/spaureliaWeb'));

}


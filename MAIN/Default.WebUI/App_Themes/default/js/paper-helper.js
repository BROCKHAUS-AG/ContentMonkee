var paperhelper = {
    computeColor: function(color, percent) {
        var num = parseInt(color.slice(1), 16), amt = Math.round(2.55 * percent), R = (num >> 16) + amt, G = (num >> 8 & 0x00FF) + amt, B = (num & 0x0000FF) + amt;
        return "#" + (0x1000000 + (R < 255 ? R < 1 ? 0 : R : 255) * 0x10000 + (G < 255 ? G < 1 ? 0 : G : 255) * 0x100 + (B < 255 ? B < 1 ? 0 : B : 255)).toString(16).slice(1);
    },
    addPointsToList: function (points, obj) {
        var segments = obj.segments;

        for (var i = 0; i < segments.length; i++) {
            points.AddUnique(points, { point: segments[i].point });
        }
    },
    movePoint: function (objects, point, newpoint) {
        var moveablesegments = [];

        for (var i = 0; i < objects.length; i++) {

            for (var x = 0; x < objects[i].segments.length; x++) {

                if (objects[i].segments[x].point.y == point.y && objects[i].segments[x].point.x == point.x) {
                    moveablesegments.push(objects[i].segments[x]);
                }
            }
        }

        for (var i = 0; i < moveablesegments.length; i++) {
            moveablesegments[i].point = newpoint;
        }

        point = newpoint;
    },
    createElements: function (paper, objects, points, height, width, basecolor) {
        var background = new paper.Path.Rectangle({
            point: [0, 0],
            size: [width * 100, height * 100]
        });
        background.sendToBack();
        background.fillColor = basecolor;

        objects[0] = new paper.Path();
        objects[0].add(new paper.Point(0, height * 28));
        objects[0].add(new paper.Point(0, height * 40));
        objects[0].add(new paper.Point(width * 12.5, height * 75));
        objects[0].add(new paper.Point(width * 8, height * 40));
        objects[0].closed = true;
        objects[0].fillColor = this.computeColor(basecolor, -5);
        this.addPointsToList(points, objects[0]);

        objects[1] = new paper.Path();
        objects[1].add(new paper.Point(width * 10, 0));
        objects[1].add(new paper.Point(width * 12, height * 12));
        objects[1].add(new paper.Point(width * 8, height * 40));
        objects[1].add(new paper.Point(width * 20, height * 60));
        objects[1].add(new paper.Point(width * 15, height * 10));
        objects[1].add(new paper.Point(width * 16.5, 0));
        objects[1].closed = true;
        objects[1].fillColor = this.computeColor(basecolor, -4);
        this.addPointsToList(points, objects[1]);

        objects[2] = new paper.Path();
        objects[2].add(new paper.Point(width * 8, height * 40));
        objects[2].add(new paper.Point(width * 12.5, height * 75));
        objects[2].add(new paper.Point(width * 25, height * 90));
        objects[2].add(new paper.Point(width * 20, height * 60));
        objects[2].closed = true;
        objects[2].fillColor = this.computeColor(basecolor, -7);
        this.addPointsToList(points, objects[2]);

        objects[3] = new paper.Path();
        objects[3].add(new paper.Point(width * 12.5, height * 75));
        objects[3].add(new paper.Point(width * 16, height * 100));
        objects[3].add(new paper.Point(width * 23, height * 100));
        objects[3].add(new paper.Point(width * 25, height * 90));
        objects[3].closed = true;
        objects[3].fillColor = this.computeColor(basecolor, -4);
        this.addPointsToList(points, objects[3]);

        objects[4] = new paper.Path();
        objects[4].add(new paper.Point(width * 20, height * 60));
        objects[4].add(new paper.Point(width * 30, height * 78));
        objects[4].add(new paper.Point(width * 30, height * 45));
        objects[4].closed = true;
        objects[4].fillColor = this.computeColor(basecolor, -7);
        this.addPointsToList(points, objects[4]);

        objects[5] = new paper.Path();
        objects[5].add(new paper.Point(width * 23, 0));
        objects[5].add(new paper.Point(width * 20, height * 9));
        objects[5].add(new paper.Point(width * 30, height * 45));
        objects[5].add(new paper.Point(width * 45, height * 9));
        objects[5].add(new paper.Point(width * 40, 0));
        objects[5].closed = true;
        objects[5].fillColor = this.computeColor(basecolor, -4);
        this.addPointsToList(points, objects[5]);

        objects[6] = new paper.Path();
        objects[6].add(new paper.Point(width * 30, height * 45));
        objects[6].add(new paper.Point(width * 30, height * 78));
        objects[6].add(new paper.Point(width * 45, height * 72));
        objects[6].closed = true;
        objects[6].fillColor = this.computeColor(basecolor, -4);
        this.addPointsToList(points, objects[6]);

        objects[7] = new paper.Path();
        objects[7].add(new paper.Point(width * 30, height * 78));
        objects[7].add(new paper.Point(width * 37, height * 100));
        objects[7].add(new paper.Point(width * 45, height * 100));
        objects[7].add(new paper.Point(width * 45, height * 72));
        objects[7].closed = true;
        objects[7].fillColor = this.computeColor(basecolor, -7);
        this.addPointsToList(points, objects[7]);

        objects[8] = new paper.Path();
        objects[8].add(new paper.Point(width * 45, 0));
        objects[8].add(new paper.Point(width * 45, height * 9));
        objects[8].add(new paper.Point(width * 52, 0));
        objects[8].closed = true;
        objects[8].fillColor = this.computeColor(basecolor, -9);
        this.addPointsToList(points, objects[8]);

        objects[9] = new paper.Path();
        objects[9].add(new paper.Point(width * 45, height * 9));
        objects[9].add(new paper.Point(width * 45, height * 72));
        objects[9].add(new paper.Point(width * 76, height * 53));
        objects[9].closed = true;
        objects[9].fillColor = this.computeColor(basecolor, -9);
        this.addPointsToList(points, objects[9]);

        objects[10] = new paper.Path();
        objects[10].add(new paper.Point(width * 45, height * 72));
        objects[10].add(new paper.Point(width * 45, height * 100));
        objects[10].add(new paper.Point(width * 67, height * 100));
        objects[10].closed = true;
        objects[10].fillColor = this.computeColor(basecolor, -10);
        this.addPointsToList(points, objects[10]);

        objects[11] = new paper.Path();
        objects[11].add(new paper.Point(width * 45, height * 72));
        objects[11].add(new paper.Point(width * 67, height * 100));
        objects[11].add(new paper.Point(width * 76, height * 53));
        objects[11].closed = true;
        objects[11].fillColor = this.computeColor(basecolor, -6);
        this.addPointsToList(points, objects[11]);

        objects[12] = new paper.Path();
        objects[12].add(new paper.Point(width * 78, 0));
        objects[12].add(new paper.Point(width * 80, height * 10));
        objects[12].add(new paper.Point(width * 76, height * 53));
        objects[12].add(new paper.Point(width * 86, height * 35));
        objects[12].add(new paper.Point(width * 84, height * 10));
        objects[12].add(new paper.Point(width * 85, 0));
        objects[12].closed = true;
        objects[12].fillColor = this.computeColor(basecolor, -5);
        this.addPointsToList(points, objects[12]);

        objects[13] = new paper.Path();
        objects[13].add(new paper.Point(width * 76, height * 53));
        objects[13].add(new paper.Point(width * 87.5, height * 42));
        objects[13].add(new paper.Point(width * 86, height * 35));
        objects[13].closed = true;
        objects[13].fillColor = this.computeColor(basecolor, -9);
        this.addPointsToList(points, objects[13]);

        objects[14] = new paper.Path();
        objects[14].add(new paper.Point(width * 76, height * 53));
        objects[14].add(new paper.Point(width * 67, height * 100));
        objects[14].add(new paper.Point(width * 91, height * 100));
        objects[14].add(new paper.Point(width * 87.5, height * 42));
        objects[14].closed = true;
        objects[14].fillColor = this.computeColor(basecolor, -13);
        this.addPointsToList(points, objects[14]);

        objects[15] = new paper.Path();
        objects[15].add(new paper.Point(width * 95, 0));
        objects[15].add(new paper.Point(width * 100, height * 10));
        objects[15].add(new paper.Point(width * 100, 0));
        objects[15].closed = true;
        objects[15].fillColor = this.computeColor(basecolor, -10);
        this.addPointsToList(points, objects[15]);

        objects[16] = new paper.Path();
        objects[16].add(new paper.Point(width * 100, height * 10));
        objects[16].add(new paper.Point(width * 86, height * 35));
        objects[16].add(new paper.Point(width * 87.5, height * 42));
        objects[16].add(new paper.Point(width * 100, height * 40));
        objects[16].closed = true;
        objects[16].fillColor = this.computeColor(basecolor, -7);
        this.addPointsToList(points, objects[16]);

        objects[17] = new paper.Path();
        objects[17].add(new paper.Point(width * 87.5, height * 42));
        objects[17].add(new paper.Point(width * 100, height * 58));
        objects[17].add(new paper.Point(width * 100, height * 40));
        objects[17].closed = true;
        objects[17].fillColor = this.computeColor(basecolor, -8);
        this.addPointsToList(points, objects[17]);

        objects[18] = new paper.Path();
        objects[18].add(new paper.Point(width * 87.5, height * 42));
        objects[18].add(new paper.Point(width * 91, height * 100));
        objects[18].add(new paper.Point(width * 100, height * 100));
        objects[18].add(new paper.Point(width * 100, height * 86));
        objects[18].closed = true;
        objects[18].fillColor = this.computeColor(basecolor, -3);
        this.addPointsToList(points, objects[18]);

        objects[19] = new paper.Path();
        objects[19].add(new paper.Point(width * 20, height * 60));
        objects[19].add(new paper.Point(width * 25, height * 90));
        objects[19].add(new paper.Point(width * 30, height * 78));
        objects[19].fillColor = this.computeColor(basecolor, -5);
        this.addPointsToList(points, objects[19]);

        objects[20] = new paper.Path();
        objects[20].add(new paper.Point(width * 25, height * 90));
        objects[20].add(new paper.Point(width * 23, height * 100));
        objects[20].add(new paper.Point(width * 29, height * 100));
        objects[20].fillColor = this.computeColor(basecolor, -8);
        this.addPointsToList(points, objects[20]);

        objects[21] = new paper.Path();
        objects[21].add(new paper.Point(width * 25, height * 90));
        objects[21].add(new paper.Point(width * 29, height * 100));
        objects[21].add(new paper.Point(width * 37, height * 100));
        objects[21].add(new paper.Point(width * 30, height * 78));
        objects[21].fillColor = this.computeColor(basecolor, -4);
        this.addPointsToList(points, objects[21]);
    },
    createAnimationPaths: function (paper, points, height, width, settings) {

        points[2].animationpath = [
            new paper.Point(width * 5, height * 60),
            new paper.Point(width * 7, height * 80),
            new paper.Point(width * 15, height * 85)
        ];

        points[3].animationpath = [
            new paper.Point(width * 4, height * 20),
            new paper.Point(width * 7, height * 35),
            new paper.Point(width * 15, height * 30)
        ];

        points[5].animationpath = [
            new paper.Point(width * 5, height * 5),
            new paper.Point(width * 7, height * 7),
            new paper.Point(width * 12, height * 20)
        ];

        points[6].animationpath = [
            new paper.Point(width * 22, height * 50),
            new paper.Point(width * 20, height * 40)
        ];

        points[7].animationpath = [
            new paper.Point(width * 18, height * 5),
            new paper.Point(width * 17.5, height * 3)
        ];

        points[9].animationpath = [
            new paper.Point(width * 27.5, height * 85),
            new paper.Point(width * 17.5, height * 80)
        ];

        points[15].animationpath = [
            new paper.Point(width * 27.5, height * 12),
            new paper.Point(width * 25, height * 5)
        ];

        points[13].animationpath = [
            new paper.Point(width * 40, height * 50),
            new paper.Point(width * 35, height * 30)
        ];

        points[12].animationpath = [
            new paper.Point(width * 35, height * 70),
            new paper.Point(width * 27.5, height * 65)
        ];

        points[16].animationpath = [
            new paper.Point(width * 55, height * 20),
            new paper.Point(width * 50, height * 10)
        ];

        points[18].animationpath = [
            new paper.Point(width * 45, height * 90),
            new paper.Point(width * 60, height * 80)
        ];

        points[23].animationpath = [
            new paper.Point(width * 70, height * 40),
            new paper.Point(width * 75, height * 70)
        ];

        points[26].animationpath = [
            new paper.Point(width * 75, height * 5),
            new paper.Point(width * 72.5, height * 20)
        ];

        points[28].animationpath = [
            new paper.Point(width * 90, height * 5),
            new paper.Point(width * 85, height * 18)
        ];

        points[27].animationpath = [
            new paper.Point(width * 85, height * 30),
            new paper.Point(width * 82.5, height * 32.5)
        ];


        points[30].animationpath = [
            new paper.Point(width * 95, height * 50),
            new paper.Point(width * 90, height * 70)
        ];

        for (var i = 0; i < points.length; i++) {
            var raw = points[i].animationpath;

            if (raw != null) {
                var animationpath = new paper.Path();

                animationpath.add(points[i].point);

                for (var x = 0; x < raw.length; x++) {
                    animationpath.add(raw[x]);
                }

                animationpath.scale(settings.intensity / 100, points[i].point);

                animationpath.closed = true;
                animationpath.smooth();

                points[i].animationpath = animationpath;
            }

            points[i].offset = 0;
        }
    },
    animate: function (event, points, objects, settings) {
        for (var i = 0; i < points.length; i++) {
            if (points[i].animationpath != null) {
                if (points[i].offset < points[i].animationpath.length) {
                    this.movePoint(objects, points[i].point, points[i].animationpath.getPointAt(points[i].offset));
                    points[i].offset += event.delta * settings.speed;
                }
                else {
                    points[i].offset = 0;
                }
            }
        }
    }
};

Array.prototype.AddUnique = function (points, point) {
    if (points.filter(function (p) {
        return p.point.x == point.point.x && p.point.y == point.point.y;
    })[0] == null) {
        points.push(point);
    }
}
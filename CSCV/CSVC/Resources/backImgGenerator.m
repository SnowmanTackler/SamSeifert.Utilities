x = 32;
b = .8;

im1 = 1 * ones([x x]);
im2 = b * ones([x x]);
im3 = [im1 im2; im2 im1];
imwrite([im3 im3; im3 im3], 'backImg.png')

x = x/2;
im1 = 1 * ones([x x]);
im2 = b * ones([x x]);
im3 = [im1 im2; im2 im1];
imwrite([im3 im3; im3 im3], 'backImgSmall.png')